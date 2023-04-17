using System;

namespace Ion.HabboHotel.Client
{
    public partial class ClientMessageHandler
    {
        /// <summary>
        /// 380 - "E|"
        /// </summary>
        private void SetFrontPageListening()
        {
            Response.Initialize(ResponseOpcodes.NavigatorFrontPageResult); // "GB"

            
            // RECOMMENDED ROOMS
            /*
            Response.AppendInt32(2); // Amount

            Response.AppendUInt32(4578541); // ID
            Response.AppendBoolean(false);
            Response.AppendString("Deep in Becca"); // Name
            Response.AppendString("Jeax"); // Owner name
            Response.AppendInt32(0); // Access type
            Response.AppendInt32(0); // curvis
            Response.AppendInt32(25); // maxvis
            Response.AppendString("Come fuck a Helsinkifag here! Rites = 9001T!");
            Response.AppendInt32(0); // All rights
            Response.AppendInt32(1); // Trading enabled
            Response.AppendInt32(9001); // Rating
            Response.AppendInt32(1); // Thumbnail
            Response.AppendString("[LOL] Mudkip category!");
            Response.AppendInt32(0);
            Response.AppendInt32(0);
            Response.AppendInt32(0);

            Response.AppendUInt32(457841); // ID
            Response.AppendBoolean(false);
            Response.AppendString("Crib"); // Name
            Response.AppendString("Nillus"); // Owner name
            Response.AppendInt32(0); // Access type
            Response.AppendInt32(0); // curvis
            Response.AppendInt32(50); // maxvis
            Response.AppendString("Lay off the bear fool!");
            Response.AppendInt32(0); // All rights
            Response.AppendInt32(0); // Trading enabled
            Response.AppendInt32(15); // Rating
            Response.AppendInt32(2); // Thumbnail
            Response.AppendString("[LOL] Mudkip category!");
            Response.AppendInt32(0);
            Response.AppendInt32(0);
            Response.AppendInt32(0);
            */

            Response.Append("IilDsVHHabbo ClubLucyIsBestHKRLWOO habbo clubHIRCITrading RoomsHHH");
            //Response.Append("IPMIPAI[KAPAHQAHRARB KTrading Rooms[xC[CHILL] Meet some new friends![{A[GAMES] Races & mazes!XdA");

            // ???
            Response.AppendInt32(1);
           // Response.AppendInt32(52);
            Response.AppendInt32(1);



            // SUBS
            // Sub amount
            Response.AppendInt32(3);

            Response.AppendInt32(1); // 'Popular rooms'
            Response.AppendInt32(0); // Amount

            Response.AppendInt32(4); // 'Where are my friends?'
            Response.AppendInt32(0); // Amount

            Response.AppendInt32(5); // 'My rooms'
            Response.AppendInt32(0); // Amount

            Response.AppendInt32(6); // 'Fav rooms'
            Response.AppendInt32(0); // Amount


            // CATEGORIES
            // Category amount
            Response.AppendInt32(1);

            Response.AppendInt32(12); // Category ID
            Response.AppendString("[JEWS] Store & breed them here!");
            Response.AppendInt32(9001); // Amount

            SendResponse();
        }

        /// <summary>
        /// 381 - "E}"
        /// </summary>
        private void GuestRoomSearch()
        {

        }

        /// <summary>
        /// 382 - "E~"
        /// </summary>
        private void GetPopularRoomTags()
        {

        }

        /// <summary>
        /// 385 - "FA"
        /// </summary>
        private void GetGuestRoom()
        {
            uint roomID = Request.PopWireduint();

            Response.Initialize(ResponseOpcodes.GetGuestRoomResult);
            Response.AppendInt32(0);
            Response.AppendUInt32(roomID); // Room ID
            Response.AppendInt32(0);
            Response.AppendString("Room 1"); // Room name
            Response.AppendString("Nillus"); // Room owner name
            Response.AppendInt32(0); // Accesstype (0 = open, 1 = closed, 2 = password)
            Response.AppendInt32(0); // Current visitor amount
            Response.AppendInt32(25); // Max visitor amount
            Response.AppendString("Description"); // Room description
            Response.AppendInt32(0);
            Response.AppendInt32(0);
            Response.AppendInt32(0);
            Response.AppendInt32(2); // Icon ID?
            Response.AppendString("Category"); // Room category name
            Response.AppendString("tag1");
            Response.AppendString("tag2");
            Response.AppendBoolean(false);
            SendResponse();
        }

        /// <summary>
        /// Registers request handlers that process room Navigator queries etc.
        /// </summary>
        public void RegisterNavigator()
        {
            mRequestHandlers[380] = new RequestHandler(SetFrontPageListening);
            mRequestHandlers[381] = new RequestHandler(GuestRoomSearch);
            mRequestHandlers[382] = new RequestHandler(GetPopularRoomTags);
            mRequestHandlers[385] = new RequestHandler(GetGuestRoom);
        }
    }
}
