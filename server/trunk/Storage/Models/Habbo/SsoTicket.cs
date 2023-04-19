using System;
using System.Text;
using System.Data;

using Ion.Storage;
using Ion.Net.Messages;

namespace Deltar.Storage.Models.Habbo
{
    /// <summary>
    /// Represents a service user's account and avatar in the account and holds the information about the account.
    /// </summary>
    public class SsoTicket
    {
        public virtual uint UserId { get; set; }
        public virtual string Ticket { get; set; }
        public virtual DateTime? ExpireDate { get; set; }
        public virtual Habbo HabboData { get; set; }
    }
}
