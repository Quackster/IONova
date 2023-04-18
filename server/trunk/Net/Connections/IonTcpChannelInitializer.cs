using DotNetty.Transport.Channels;
using Ion.Net.Connections.Codec;

namespace Ion.Net.Connections
{
    internal class IonTcpChannelInitializer : ChannelInitializer<IChannel>
    {
        #region Fields 

        private IonTcpConnectionFactory mFactory;

        #endregion

        #region Constructor

        public IonTcpChannelInitializer(IonTcpConnectionFactory pFactory)
        {
            mFactory = pFactory;
        }

        #endregion

        protected override void InitChannel(IChannel channel)
        {
            IChannelPipeline pipeline = channel.Pipeline;
            pipeline.AddLast("gameEncoder", new NetworkEncoder());
            pipeline.AddLast("gameDecoder", new NetworkDecoder());
            pipeline.AddLast("clientHandler", new IonTcpNetworkHandler(mFactory));
        }
    }
}