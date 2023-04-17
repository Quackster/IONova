using System;
using System.Data;
using System.Collections.Generic;

using Ion.Storage;
using Ion.HabboHotel.Client;

namespace Ion.HabboHotel.Messenger
{
    public class MessengerComponent
    {
        #region Fields
        private GameClient mClient;
        private List<MessengerBuddy> mBuddies;
        #endregion

        #region Constructors
        public MessengerComponent(GameClient client)
        {
            mClient = client;
            mBuddies = new List<MessengerBuddy>();
        }
        #endregion

        #region Methods
        public void ReloadBuddies()
        {
            mBuddies.Clear();
            using (DatabaseClient dbClient = IonEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("@userid", mClient.GetHabbo().ID);
                foreach (DataRow row in dbClient.ReadDataTable("SELECT id,username,figure,motto FROM users WHERE id IN(SELECT buddyid FROM messenger_buddylist WHERE userid = @userid AND accepted = 0x01) OR id IN(SELECT userid FROM messenger_buddylist WHERE buddyid = @userid AND accepted = 0x01);").Rows)
                {
                    MessengerBuddy buddy = MessengerBuddy.Parse(row);
                    if (buddy != null)
                    {
                        mBuddies.Add(buddy);
                    }
                }
            }
        }
        public List<MessengerBuddy> GetBuddies()
        {
            return mBuddies;
        }

        public MessengerBuddy GetBuddy(uint buddyID)
        {
            lock (mBuddies)
            {
                foreach (MessengerBuddy buddy in mBuddies)
                {
                    if (buddy.ID == buddyID)
                    {
                        return buddy;
                    }
                }
            }

            return null;
        }

        #endregion
    }
}
