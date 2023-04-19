using System;
using System.Text;
using System.Data;

using Ion.Storage;
using Ion.Net.Messages;
using System.Collections.Generic;

namespace Deltar.Storage.Models.Habbo
{
    /// <summary>
    /// Represents a service user's account and avatar in the account and holds the information about the account.
    /// </summary>
    public class Habbo : ISerializableObject
    {
        public virtual uint Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Figure { get; set; }
        public virtual string Gender { get; set; }
        public virtual int Rank { get; set; }
        public virtual uint Credits { get; set; }
        public virtual uint ActivityPoints { get; set; }
        public virtual DateTime JoinDate { get; set; }
        public virtual DateTime LastOnline { get; set; }
        public virtual string Motto { get; set; }
        public virtual List<SsoTicket> Tickets { get; set; }

        #region Methods

        public void Serialize(ServerMessage message)
        {
            message.AppendString(Id.ToString());
            message.AppendString(Name);
            message.AppendString(Figure);
            message.AppendString(Gender.ToString());
            message.AppendString(Motto.ToString());
            message.AppendBoolean(false);
            message.AppendString("");
            message.AppendBoolean(false);
            message.AppendBoolean(false);
            message.AppendBoolean(false);
            message.AppendBoolean(false);
        }

        #endregion
    }
}
