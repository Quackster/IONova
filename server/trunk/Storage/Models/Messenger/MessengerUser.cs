namespace Ion.Storage.Models
{
    public class MessengerUser
    {
        public virtual uint UserId { get; set; }
        public virtual uint FriendId { get; set; }
        public virtual Habbo FriendData { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var t = obj as MessengerUser;

            if (t == null)
                return false;

            if (FriendId == t.FriendId &&
                UserId == t.UserId)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
