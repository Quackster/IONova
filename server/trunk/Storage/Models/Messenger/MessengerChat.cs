using System;

namespace Ion.Storage.Models
{
    public class MessengerChat
    {
        public virtual uint UserId { get; set; }
        public virtual uint FriendId { get; set; }
        public virtual string Message { get; set; }
        public virtual bool IsRead { get; set; }
        public virtual DateTime MessagedAt { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var t = obj as MessengerChat;

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
