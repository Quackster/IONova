using System;
using System.Linq;
using Ion.Storage.Models;
using Ion.HabboHotel.Client;
using Microsoft.EntityFrameworkCore;

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

                /*
                if (habbo.LoadByID(IonEnvironment.GetDatabase(), ID))
                {
                    return habbo;
                }
                */
            }

            return null;
        }
        public Habbo GetHabbo(string sUsername)
        {
            // TODO: some sort of cache?

            Habbo habbo = new Habbo();

            /*
            if (habbo.LoadByUsername(IonEnvironment.GetDatabase(), sUsername))
            {
                return habbo;
            }*/

            return null;
        }

        public bool UpdateHabbo(Habbo habbo)
        {
            return true;
        }

        public bool LoadBySsoTicket(out Habbo habbo, string sSsoTicket)
        {
            using (var context = IonEnvironment.GetDatabase().GetContext())
            {
                var row = context.SsoTicket.Include(x => x.HabboData).Where(x =>
                       (x.HabboData != null && x.Ticket == sSsoTicket) &&
                       (x.ExpireDate == null || x.ExpireDate > DateTime.Now))
                   .Take(1)
                   .SingleOrDefault();

                habbo = row?.HabboData;
            }

            return habbo != null;
        }

        public bool LoadHabboByUsername(out Habbo habbo, string sUsername)
        {
            using (var context = IonEnvironment.GetDatabase().GetContext())
            {
               habbo = context.Habbo.FirstOrDefault(x => x.Name == sUsername);
            }

            return habbo != null;
        }

        #endregion
    }
}
