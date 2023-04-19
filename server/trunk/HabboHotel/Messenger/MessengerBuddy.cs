using Ion;
using Ion.HabboHotel.Client;
using Ion.Net.Messages;
using Ion.Storage.Models;
using System.Security.Cryptography;

namespace Ion.HabboHotel.Messenger
{
    public class MessengerBuddy : ISerializableObject
    {
        #region Properties

        public Habbo Habbo
        {
            get; set;
        }

        public GameClient Client
        {
            get { return IonEnvironment.GetHabboHotel().GetClients().GetClientOfHabbo(Habbo.Id); }
        }

        public bool IsOnline
        {
            get { return Client != null; }
        }

        public bool InRoom => false;

        #endregion

        #region Constructors

        public MessengerBuddy(Habbo friendData)
        {
            Habbo = friendData;
        }

        #endregion

        #region Public methods

        public void Serialize(ServerMessage message)
        {
            message.AppendUInt32(Habbo.Id);
            message.AppendString(Habbo.Name);
            message.AppendBoolean(true);
            message.AppendBoolean(true);
            message.AppendBoolean(false);
            message.AppendString(Habbo.Figure);
            message.AppendBoolean(false);
            message.AppendString(Habbo.Motto);
            message.AppendString("1-1-1970");
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var t = obj as MessengerBuddy;

            if (t == null || t.Habbo == null || Habbo == null)
                return false;

            if (t.Habbo.Id == Habbo.Id)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }
}
