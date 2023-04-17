using System;

namespace Ion.HabboHotel.Client
{
    public partial class ClientMessageHandler
    {
        /// <summary>
        /// 196 - "CD"
        /// </summary>
        private void Pong()
        {
            mSession.PingOK = true;
        }

        /// <summary>
        /// Registers request handlers available from start of client.
        /// </summary>
        public void RegisterGlobal()
        {
            mRequestHandlers[196] = new RequestHandler(Pong);
        }
    }
}
