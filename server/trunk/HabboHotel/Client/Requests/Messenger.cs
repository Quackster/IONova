using System;
using System.Collections.Generic;

using Ion.Net.Messages;
using Ion.HabboHotel.Messenger;

namespace Ion.HabboHotel.Client
{
    public partial class ClientMessageHandler
    {
        /// <summary>
        /// 15 - "@O"
        /// </summary>
        private void FriendListUpdate()
        {

        }

        /// <summary>
        /// 33 - "@a"
        /// </summary>
        private void SendMsg()
        {
            uint buddyID = Request.PopWireduint();
            string sText = Request.PopFixedString();

            // Buddy in list?
            if (mSession.GetMessenger().GetBuddy(buddyID) != null)
            {
                // Buddy online?
                GameClient buddyClient = IonEnvironment.GetHabboHotel().GetClients().GetClientOfHabbo(buddyID);
                if (buddyClient == null)
                {
                    Response.Initialize(ResponseOpcodes.InstantMessageError); // Opcode
                    Response.AppendInt32(5); // Error code
                    Response.AppendUInt32(mSession.GetHabbo().ID);
                    SendResponse();
                }
                else
                {
                    ServerMessage notify = new ServerMessage(ResponseOpcodes.NewConsole);
                    notify.AppendUInt32(mSession.GetHabbo().ID);
                    notify.AppendString(sText);
                    buddyClient.GetConnection().SendMessage(notify);
                }
            }
        }

        /// <summary>
        /// 34 - "@b"
        /// </summary>
        private void SendRoomInvite()
        {
            // TODO: check if this session is in room

            // Determine how many receivers
            int amount = Request.PopWiredInt32();
            List<GameClient> receivers = new List<GameClient>(amount);

            // Get receivers
            for (int i = 0; i < amount; i++)
            {
                // User in buddy list?
                uint buddyID = Request.PopWireduint();
                if (mSession.GetMessenger().GetBuddy(buddyID) != null)
                {
                    // User online?
                    GameClient buddyClient = IonEnvironment.GetHabboHotel().GetClients().GetClientOfHabbo(buddyID);
                    if (buddyClient != null)
                    {
                        receivers.Add(buddyClient);
                    }
                }
            }

            // Parse text
            string sText = Request.PopFixedString();

            // Notify the receivers
            ServerMessage notify = new ServerMessage(ResponseOpcodes.RoomInvite);
            //...
            foreach (GameClient receiver in receivers)
            {
                receiver.GetConnection().SendMessage(notify);
            }
        }

        /// <summary>
        /// 37 - "@e"
        /// </summary>
        private void AcceptBuddy()
        {
            int amount = Request.PopWiredInt32();
            for (int i = 0; i < amount; i++)
            {
                uint buddyID = Request.PopWireduint();
            }
        }

        /// <summary>
        /// 38 - "@f"
        /// </summary>
        private void DeclineBuddy()
        {
            int amount = Request.PopWiredInt32();
            for (int i = 0; i < amount; i++)
            {
                uint buddyID = Request.PopWireduint();
            }
        }
        
        /// <summary>
        /// 39 - "@g"
        /// </summary>
        private void RequestBuddy()
        {

        }

        /// <summary>
        /// 40 - "@h"
        /// </summary>
        private void RemoveBuddy()
        {
            uint amount = Request.PopUInt32();
            for (int i = 0; i < amount; i++)
            {
                uint buddyID = Request.PopWireduint();
                if (mSession.GetMessenger().GetBuddy(buddyID) != null)
                {

                }
            }
        }

        /// <summary>
        /// 41 - "@i"
        /// </summary>
        private void HabboSearch()
        {
            GetCatalogIndex();
            return;

            // Parse search criteria
            string sCriteria = Request.PopFixedString();
            sCriteria = sCriteria.Replace("%", "");

            // Query Habbos with names similar to criteria
            List<MessengerBuddy> matches = IonEnvironment.GetHabboHotel().GetMessenger().SearchHabbos(sCriteria);

            // Build response
            Response.Initialize(ResponseOpcodes.HabboSearchResult);
            Response.AppendInt32(matches.Count);
            foreach(MessengerBuddy match in matches)
            {
                //...
            }
            SendResponse();
        }

        /// <summary>
        /// 233 - "Ci"
        /// </summary>
        private void GetBuddyRequests()
        {
            Response.Initialize(ResponseOpcodes.BuddyRequests); // "Dz"
            
            Response.AppendInt32(3); // Amount
            Response.AppendInt32(3); // Amount
            
            Response.AppendInt32(123456); // User ID
            Response.AppendString("HybridCore.NET");
            Response.AppendString("5"); // Request ID

            Response.AppendInt32(123423); // User ID
            Response.AppendString("Jordan");
            Response.AppendString("7"); // Request ID

            Response.AppendInt32(1234231337); // User ID
            Response.AppendString("Nillus");
            Response.AppendString("12"); // Request ID

            SendResponse();
        }

        /// <summary>
        /// 262 - "DF"
        /// </summary>
        private void FollowFriend()
        {

        }

        /// <summary>
        /// Registers the request handlers for the in-game messenger. ('Console')
        /// </summary>
        public void RegisterMessenger()
        {
            mRequestHandlers[15] = new RequestHandler(FriendListUpdate);
            mRequestHandlers[33] = new RequestHandler(SendMsg);
            mRequestHandlers[34] = new RequestHandler(SendRoomInvite);
            mRequestHandlers[37] = new RequestHandler(AcceptBuddy);
            mRequestHandlers[38] = new RequestHandler(DeclineBuddy);
            mRequestHandlers[39] = new RequestHandler(RequestBuddy);
            mRequestHandlers[40] = new RequestHandler(RemoveBuddy);
            mRequestHandlers[41] = new RequestHandler(HabboSearch);
            mRequestHandlers[233] = new RequestHandler(GetBuddyRequests);
            mRequestHandlers[262] = new RequestHandler(FollowFriend);
        }
    }
}
