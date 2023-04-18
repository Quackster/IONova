using DotNetty.Buffers;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Transport.Channels;
using System;
using System.Net;
using DotNetty.Common.Utilities;
using System.Threading;
using System.Threading.Tasks;
using Ion.Net.Messages;

namespace Ion.Net.Connections
{
    /// <summary>
    /// Listens for TCP connections at a given port, asynchronously accepting connections and optionally insert them in the Ion environment connection manager.
    /// </summary>
    public class IonTcpConnectionListener1
    {
        #region Fields
        /// <summary>
        /// The maximum length of the connection request queue for the listener as an integer.
        /// </summary>
        private const int LISTENER_CONNECTIONREQUEST_QUEUE_LENGTH = 1;

        private bool mIsListening = false;
        private IonTcpConnectionManager mManager;

        /// <summary>
        /// An IonTcpConnectionFactory instance that is capable of creating IonTcpConnections.
        /// </summary>
        private IonTcpConnectionFactory mFactory;

        private MultithreadEventLoopGroup mBossGroup;
        private MultithreadEventLoopGroup mWorkerGroup;

        #endregion

        #region Properties
        /// <summary>
        /// Gets whether the listener is listening for new connections or not.
        /// </summary>
        public bool isListening
        {
            get { return mIsListening; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs an IonTcpConnection listener and binds it to a given local IP address and TCP port.
        /// </summary>
        /// <param name="sLocalIP">The IP address string to parse and bind the listener to.</param>
        /// <param name="Port">The TCP port number to parse the listener to.</param>
        public IonTcpConnectionListener1(string sLocalIP, int Port, IonTcpConnectionManager pManager)
        {
            IPAddress pIP = null;
            if(!IPAddress.TryParse(sLocalIP, out pIP))
            {
                pIP = IPAddress.Loopback;
                IonEnvironment.GetLog().WriteWarning(string.Format("Connection listener was unable to parse the given local IP address '{0}', now binding listener to '{1}'.", sLocalIP, pIP.ToString())); 
            }

            mFactory = new IonTcpConnectionFactory();
            mManager = pManager;

            mBossGroup = new MultithreadEventLoopGroup(1);
            mWorkerGroup = new MultithreadEventLoopGroup(10);

            ServerBootstrap bootstrap = new ServerBootstrap()
                .Group(mBossGroup, mWorkerGroup)
                .Channel<TcpServerSocketChannel>()
                .ChildHandler(new IonTcpChannelInitializer(mFactory))
                .ChildOption(ChannelOption.TcpNodelay, true)
                .ChildOption(ChannelOption.SoKeepalive, true)
                .ChildOption(ChannelOption.SoReuseaddr, true)
                .ChildOption(ChannelOption.SoRcvbuf, 1024)
                .ChildOption(ChannelOption.RcvbufAllocator, new FixedRecvByteBufAllocator(1024))
                .ChildOption(ChannelOption.Allocator, UnpooledByteBufferAllocator.Default);

            bootstrap.BindAsync(pIP, Port);

            IonEnvironment.GetLog().WriteLine(string.Format("IonTcpConnectionListener initialized and bound to {0}:{1}.", pIP.ToString(), Port.ToString()));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Starts listening for connections.
        /// </summary>
        public void Start()
        {
            if (mIsListening)
                return;

            mIsListening = true;
        }
        /// <summary>
        /// Stops listening for connections.
        /// </summary>
        public void Stop()
        {
            if (!mIsListening)
                return;

            mIsListening = false;

            mBossGroup.ShutdownGracefullyAsync();
            mWorkerGroup.ShutdownGracefullyAsync();
        }
        /// <summary>
        /// Destroys all resources in the connection listener.
        /// </summary>
        public void Destroy()
        {
            Stop();

            mManager = null;
            mFactory = null;
        }

        #endregion
    }

    internal class IonTcpNetworkHandler : ChannelHandlerAdapter
    {
        #region Fields

        public static AttributeKey<IonTcpConnection> CONNECTION_KEY = AttributeKey<IonTcpConnection>.NewInstance("CONNECTION_KEY");
        private IonTcpConnectionFactory mFactory;

        #endregion

        #region Constructor

        public IonTcpNetworkHandler(IonTcpConnectionFactory pFactory)
        {
            mFactory = pFactory;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Handle client connections.
        /// </summary>
        /// <param name="ctx">the channel context</param>
        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            base.ChannelActive(ctx);

            var connection = mFactory.CreateConnection(ctx.Channel);

            IonEnvironment.GetTcpConnections().HandleNewConnection(connection);

            if (connection != null)
            {
                 ctx.Channel.GetAttribute(CONNECTION_KEY).SetIfAbsent(connection);
            }
        }

        /// <summary>
        /// Handle client disconnects.
        /// </summary>
        /// <param name="ctx">the channel context</param>
        public override void ChannelInactive(IChannelHandlerContext ctx)
        {
            base.ChannelInactive(ctx);
            IonTcpConnection connection = ctx.Channel.GetAttribute(CONNECTION_KEY).Get();

            if (connection == null)
                return;

            connection.Stop();
        }

        /// <summary>
        /// Handle incoming channel messages from the decoder
        /// </summary>
        /// <param name="ctx">the channel context</param>
        /// <param name="msg">the incoming message</param>
        public override void ChannelRead(IChannelHandlerContext ctx, object msg)
        {
            IonTcpConnection connectionSession = ctx.Channel.GetAttribute(CONNECTION_KEY).Get();

            if (connectionSession == null)
                return;

            if (msg is ClientMessage clientMessage)
                connectionSession.RouteData(clientMessage);

            base.ChannelRead(ctx, msg);
        }

        /// <summary>
        /// Handle channel read complete.
        /// </summary>
        /// <param name="context">the channel context</param>
        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        /// <summary>
        /// Handle exceptions thrown by the network api.
        /// </summary>
        /// <param name="context">the channel context</param>
        /// <param name="exception">the exception</param>
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) =>
            IonEnvironment.GetLog().WriteError(exception.ToString());

        #endregion
    }
}
