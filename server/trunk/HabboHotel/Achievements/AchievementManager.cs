using System;
using System.Data;
using System.Collections.Generic;

using Ion.Storage;

namespace Ion.HabboHotel.Achievements
{
    /// <summary>
    /// Provides management for the achievement that Habbos can receive.
    /// </summary>
    public class AchievementManager
    {
        #region Methods
        public List<string> GetAchievements(uint userID)
        {
            List<string> achievements = new List<string>();
            using (DatabaseClient dbClient = IonEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("@userid", userID);
                foreach (DataRow row in dbClient.ReadDataTable("SELECT achievement FROM users_achievements WHERE userid = @userid;").Rows)
                {
                    achievements.Add((string)row["achievement"]);
                }
            }

            return achievements;
        }

        public void AddAchievement(uint userID, string sAchievement)
        {
            using (DatabaseClient dbClient = IonEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("@userid", userID);
                dbClient.AddParamWithValue("@achievement", sAchievement);
                dbClient.ExecuteQuery("INSERT INTO users_achievements(userid,achievement) VALUES (@userid,@achievement);");
            }
        }

        public void RemoveAchievement(uint userID, string sAchievement)
        {
            using (DatabaseClient dbClient = IonEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("@userid", userID);
                dbClient.AddParamWithValue("@achievement", sAchievement);
                dbClient.ExecuteQuery("DELETE FROM users_achievements WHERE userid = @userid AND achievement = @achievement;");
            }
        }

        public bool HasAchievement(uint userID, string sAchievement)
        {
            using (DatabaseClient dbClient = IonEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("@userid", userID);
                dbClient.AddParamWithValue("@achievement", sAchievement);
                return (dbClient.ReadInt32("SELECT COUNT(userID) FROM users_achievements WHERE userid = @userid AND achievement = @achievement LIMIT 1;") > 0);
            }
        }
        #endregion
    }
}
