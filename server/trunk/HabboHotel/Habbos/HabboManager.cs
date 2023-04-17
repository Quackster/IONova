using System;

using Ion.HabboHotel.Client;

namespace Ion.HabboHotel.Habbos
{
    /// <summary>
    /// Manages service users ('Habbo's') and provides methods for updating and retrieving accounts.
    /// </summary>
    public class HabboManager
    {
        #region Constructors

        #endregion

        #region Methods
        public Habbo GetHabbo(uint ID)
        {
            // Prefer active client over Database
            GameClient client = IonEnvironment.GetHabboHotel().GetClients().GetClientOfHabbo(ID);
            if (client != null)
            {
                return client.GetHabbo();
            }
            else
            {
                Habbo habbo = new Habbo();
                if (habbo.LoadByID(IonEnvironment.GetDatabase(), ID))
                {
                    return habbo;
                }
            }

            return null;
        }
        public Habbo GetHabbo(string sUsername)
        {
            // TODO: some sort of cache?

            Habbo habbo = new Habbo();
            if (habbo.LoadByUsername(IonEnvironment.GetDatabase(), sUsername))
            {
                return habbo;
            }

            return null;
        }

        public bool UpdateHabbo(Habbo habbo)
        {
            return IonEnvironment.GetDatabase().UPDATE(habbo);
        }
        #endregion
    }
}
