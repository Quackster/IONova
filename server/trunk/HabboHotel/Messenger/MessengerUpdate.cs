using Ion.Storage.Models;

namespace Ion.HabboHotel.Messenger
{
    public enum MessengerUpdateType
    {
        RemoveFriend = -1,
        UpdateFriend = 0,
        AddFriend = 1
    }

    public class MessengerUpdate
    {
        public MessengerUpdateType UpdateType { get; set; }

        public MessengerBuddy Friend { get; set; }
    }
}