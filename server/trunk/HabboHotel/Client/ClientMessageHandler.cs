using Ion.Net.Messages;

namespace Ion.HabboHotel.Client
{
    public partial class ClientMessageHandler
    {
        #region Fields
        private const int HIGHEST_MESSAGEID = 2002; // class="com.sulake.habbo.communication.messages.outgoing.handshake::GenerateSecretKeyMessageComposer" />
        private GameClient mSession;

        private ClientMessage Request;
        private ServerMessage Response;

        private delegate void RequestHandler();
        private RequestHandler[] mRequestHandlers;
        #endregion

        #region Constructor
        public ClientMessageHandler(GameClient pSession)
        {
            mSession = pSession;
            mRequestHandlers = new RequestHandler[HIGHEST_MESSAGEID + 1];
            
            Response = new ServerMessage(0);
        }
        #endregion

        #region Methods
        public ServerMessage GetResponse()
        {
            return Response;
        }

        /// <summary>
        /// Destroys all the resources in the ClientMessageHandler.
        /// </summary>
        public void Destroy()
        {
            mSession = null;
            mRequestHandlers = null;

            Request = null;
            Response = null;
        }
        /// <summary>
        /// Invokes the matching request handler for a given ClientMessage.
        /// </summary>
        /// <param name="request">The ClientMessage object to process.</param>
        public void HandleRequest(ClientMessage request)
        {
            IonEnvironment.GetLog().WriteLine("[" + mSession.ID + "] --> " + request.Header + request.GetContentString());

            if (request.ID > HIGHEST_MESSAGEID)
                return; // Not in protocol
            if (mRequestHandlers[request.ID] == null)
                return; // Handler not registered

            // Handle request
            Request = request;
            mRequestHandlers[request.ID].Invoke();
            Request = null;
        }
        /// <summary>
        /// Sends the current response ServerMessage on the stack.
        /// </summary>
        public void SendResponse()
        {
            if (Response.ID > 0)
            {
                mSession.GetConnection().SendMessage(Response);
            }
        }
        #endregion
    }
}
