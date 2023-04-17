using System;
using System.Data;
using System.Collections.Generic;

using Ion.Storage;

namespace Ion.HabboHotel.Messenger
{
    public class MessengerManager
    {
        public List<MessengerBuddy> SearchHabbos(string criteria)
        {
            List<MessengerBuddy> matches = new List<MessengerBuddy>();
            using (DatabaseClient dbClient = IonEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("@criteria", criteria + "%");
                foreach (DataRow row in dbClient.ReadDataTable("SELECT id,username,figure,motto FROM users WHERE username LIKE @criteria;").Rows)
                {
                    MessengerBuddy match = MessengerBuddy.Parse(row);
                    if (match != null)
                    {
                        matches.Add(match);
                    }
                }
            }

            return matches;
        }
    }
}
