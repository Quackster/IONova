using System;
using System.Data;

using Ion.Storage;
using Ion.Net.Messages;

namespace Ion.HabboHotel.Rooms
{
    public class Room : ISerializableObject
    {
        #region Fields
        private uint mID;
        private uint mOwnerID;
        private string mOwnerName;

        private string mName;
        private string mDescription;
        private RoomAccessType mAccessType;
        private string mPassword;

        private ushort mVisitors;
        private ushort mMaxVisitors;
        #endregion

        #region Properties
        public uint ID
        {
            get { return mID; }
        }
        public uint OwnerID
        {
            get { return mOwnerID; }
        }
        public string OwnerName
        {
            get { return mOwnerName; }
        }
        public string Name
        {
            get { return mName; }
            set
            {
                mName = value;
            }
        }
        public string Description
        {
            get { return mDescription; }
            set
            {
                mDescription = value;
            }
        }

        public ushort Visitors
        {
            get { return mVisitors; }
        }
        public ushort MaxVisitors
        {
            get { return mVisitors; }
        }
        #endregion

        #region Methods
        public void Serialize(ServerMessage message)
        {
            message.AppendUInt32(mID);
            message.AppendBoolean(false);
            message.AppendString(mName);
            message.AppendString(mOwnerName);
            message.AppendInt32((int)mAccessType);
            message.AppendInt32((int)mVisitors);
            message.AppendInt32((int)mMaxVisitors);
            message.AppendString(mDescription);
            message.AppendBoolean(false); // All rights
            message.AppendBoolean(false); // Allow trading
        }
        #region Storage
        public void INSERT(DatabaseManager database)
        {

        }
        public void DELETE(DatabaseManager database)
        {

        }
        #endregion
        #endregion
    }
}
