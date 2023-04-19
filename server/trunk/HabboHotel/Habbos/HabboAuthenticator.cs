using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Deltar.Storage.Models.Habbo;

namespace Ion.HabboHotel.Habbos
{
    public class HabboAuthenticator
    {
        #region Constructor
        
        #endregion

        #region Methods
        public Habbo Login(string sUsername, string sPassword)
        {
            // Do not use HabboManager.GetHabbo(string) here, as caching is planned to be implemented there
            Habbo habbo = new Habbo();
            if (IonEnvironment.GetHabboHotel().GetHabbos().LoadHabboByUsername(out habbo, sUsername))
                throw new IncorrectLoginException("login incorrect: Wrong username");

            // if (habbo.Password != sPassword)
            //    throw new IncorrectLoginException("login incorrect: Wrong password");

            // Drop old client (if logged in via other connection)
            IonEnvironment.GetHabboHotel().GetClients().KillClientOfHabbo(habbo.Id);

            return habbo;
        }
        public Habbo Login(string sTicket)
        {
            // Do not use HabboManager.GetHabbo(string) here, as caching is planned to be implemented there
            Habbo habbo = new Habbo();
            if (!IonEnvironment.GetHabboHotel().GetHabbos().LoadBySsoTicket(out habbo, sTicket))
            {
                throw new IncorrectLoginException("login incorrect: Wrong ticket");
            }
            else
            {
                // Drop old client (if logged in via other connection)
                IonEnvironment.GetHabboHotel().GetClients().KillClientOfHabbo(habbo.Id);

                return habbo;
            }
        }
        #endregion
    }
}
