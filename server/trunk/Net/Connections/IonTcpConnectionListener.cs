using System;
using System.Net;
using System.Net.Sockets;

namespace Ion.Net.Connections
{
    /// <summary>
    /// Listens for TCP connections at a given port, asynchronously accepting connections and optionally insert them in the Ion environment connection manager.
    /// </summary>
    public class IonTcpConnectionListener
    {
        #region Fields
        /// <summary>
        /// The maximum length of the connection request queue for the listener as an integer.
        /// </summary>
        private const int LISTENER_CONNECTIONREQUEST_QUEUE_LENGTH = 1;

        /// <summary>
        /// A System.Net.Sockets.TcpListener that listens for connections.
        /// </summary>
        private TcpListener mListener = null;
        private bool mIsListening = false;
        private AsyncCallback mConnectionRequestCallback = null;

        private IonTcpConnectionManager mManager;
        /// <summary>
        /// An IonTcpConnectionFactory instance that is capable of creating IonTcpConnections.
        /// </summary>
        private IonTcpConnectionFactory mFactory;
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
        public IonTcpConnectionListener(string sLocalIP, int Port, IonTcpConnectionManager pManager)
        {
            IPAddress pIP = null;
            if(!IPAddress.TryParse(sLocalIP, out pIP))
            {
                pIP = IPAddress.Loopback;
                IonEnvironment.GetLog().WriteWarning(string.Format("Connection listener was unable to parse the given local IP address '{0}', now binding listener to '{1}'.", sLocalIP, pIP.ToString())); 
            }

            mListener = new TcpListener(pIP, Port);
            mConnectionRequestCallback = new AsyncCallback(ConnectionRequest);
            mFactory = new IonTcpConnectionFactory();
            mManager = pManager;

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

            mListener.Start();
            mIsListening = true;

            WaitForNextConnection();
        }
        /// <summary>
        /// Stops listening for connections.
        /// </summary>
        public void Stop()
        {
            if (!mIsListening)
                return;

            mIsListening = false;
            mListener.Stop();
        }
        /// <summary>
        /// Destroys all resources in the connection listener.
        /// </summary>
        public void Destroy()
        {
            Stop();

            mListener = null;
            mManager = null;
            mFactory = null;
        }

        /// <summary>
        /// Waits for the next connection request in it's own thread.
        /// </summary>
        private void WaitForNextConnection()
        {
            if (mIsListening)
                mListener.BeginAcceptSocket(mConnectionRequestCallback, null);
        }
        /// <summary>
        /// Invoked when the listener asynchronously accepts a new connection request.
        /// </summary>
        /// <param name="iAr">The IAsyncResult object holding the results of the asynchronous BeginAcceptSocket operation.</param>
        private void ConnectionRequest(IAsyncResult iAr)
        {
            try
            {
                Socket pSocket = mListener.EndAcceptSocket(iAr);
                // TODO: IP blacklist

                IonTcpConnection connection = mFactory.CreateConnection(pSocket);
                if (connection != null)
                {
                    mManager.HandleNewConnection(connection);
                }
            }
            catch { } // TODO: handle exceptions
            finally
            {
                if (mIsListening)
                    WaitForNextConnection(); // Re-start the process for next connection
            }
        }
        #endregion
    }
}
