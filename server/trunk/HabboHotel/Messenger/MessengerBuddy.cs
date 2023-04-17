using System;
using System.Data;

using Ion.Storage;
using Ion.Net.Messages;

namespace Ion.HabboHotel.Messenger
{
    public class MessengerBuddy : ISerializableObject
    {
        #region Fields
        private uint mID;
        private string mUsername;
        private string mFigure;
        private string mMotto;
        #endregion

        #region Properties
        public uint ID
        {
            get { return mID; }
        }
        public string Username
        {
            get { return mUsername; }
        }
        public string Figure
        {
            get { return mFigure; }
        }
        #endregion

        #region Methods
        public void Serialize(ServerMessage message)
        {
            message.AppendUInt32(mID);
            message.AppendString(mUsername);
            message.AppendBoolean(true);
            message.AppendBoolean(true);
            message.AppendBoolean(false);
            message.AppendString(mFigure);
            message.AppendBoolean(false);
            message.AppendString(mMotto);
            message.AppendString("1-1-1970");
        }

        public static MessengerBuddy Parse(DataRow row)
        {
            MessengerBuddy buddy = new MessengerBuddy();
            try
            {
                buddy.mID = (uint)row["id"];
                buddy.mUsername = (string)row["username"];
                buddy.mFigure = (string)row["figure"];
                buddy.mMotto = (string)row["motto"];

                return buddy;
            }
            catch (Exception ex)
            {
                IonEnvironment.GetLog().WriteUnhandledExceptionError("MessengerBuddy.Parse", ex);
            }

            return null;
        }
        #endregion
    }
}
