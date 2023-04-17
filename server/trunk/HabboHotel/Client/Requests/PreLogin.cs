using System;

namespace Ion.HabboHotel.Client
{
    public partial class ClientMessageHandler
    {
        private void InitCrypto()
        {
            Response.Initialize(ResponseOpcodes.SessionParams); // "DA"
            Response.AppendInt32(9);
            Response.AppendInt32(0);
            Response.AppendInt32(0);
            Response.AppendInt32(1);
            Response.AppendInt32(1);
            Response.AppendInt32(3);
            Response.AppendInt32(0);
            Response.AppendInt32(2);
            Response.AppendInt32(1);
            Response.AppendInt32(4);
            Response.AppendInt32(0);
            Response.AppendInt32(5);
            Response.AppendString("dd-MM-yyyy");
            Response.AppendInt32(7);
            Response.AppendBoolean(false);
            Response.AppendInt32(8);
            Response.AppendString("hotel-co.uk");
            Response.AppendInt32(9);
            Response.AppendBoolean(false);
            SendResponse();

            /* WITH ENCRYPTION
            bool status = Request.PopWiredBoolean();
            string token = "02d6b0969196802df2";

            Response.Initialize(ResponseOpcodes.InitCrypto); // "DU"
            Response.AppendString(token);
            Response.AppendBoolean(status);
            SendResponse();
            */
        }

        private void SSOTicket()
        {
            // Process ticket login
            string sTicket = Request.PopFixedString();
            mSession.Login(sTicket);
        }

        private void GenerateSecretKey()
        {
            // Get the client's public key (by the token)
            string sClientKey = Request.PopFixedString();

            // Now create the server's public key (we love Diffie Hellman!)
            string sServerKey = new Random().Next(0, int.MaxValue).ToString();

            // And send the server's public key
            Response.Initialize(ResponseOpcodes.SecretKey); // "@A"
            Response.Append(sServerKey);
            SendResponse();
        }

        public void RegisterPreLogin()
        {
            mRequestHandlers[206] = new RequestHandler(InitCrypto);
            mRequestHandlers[415] = new RequestHandler(SSOTicket);
            mRequestHandlers[2002] = new RequestHandler(GenerateSecretKey);

        }
        public void UnRegisterPreLogin()
        {
            mRequestHandlers[206] = null;
            mRequestHandlers[415] = null;
            mRequestHandlers[2002] = null;
        }
    }
}
