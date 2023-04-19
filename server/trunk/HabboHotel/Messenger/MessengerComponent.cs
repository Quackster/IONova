using Ion.HabboHotel.Client;
using Ion.Specialized.Utilities.Extensions;
using Ion.Storage.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Ion.HabboHotel.Messenger
{
    public class MessengerComponent
    {

        #region Properties

        /// <summary>
        /// Get friend requests
        /// </summary>
        public List<MessengerBuddy> Requests { get; private set; }

        /// <summary>
        /// Get friends
        /// </summary>
        public List<MessengerBuddy> Friends { get; private set; }

        /// <summary>
        /// Get categories
        /// </summary>
        public List<MessengerCategory> Categories { get; private set; }

        /// <summary>
        /// Get concurrent messenger update queue
        /// </summary>
        public ConcurrentQueue<MessengerUpdate> Queue { get; private set; }

        /// <summary>
        /// Get the maximum friends allowed
        /// </summary>
        public int MaxFriendsAllowed => 600;

        /// <summary>
        /// Get the player for this messenger instance
        /// </summary>
        public GameClient Player { get; set; }

        /// <summary>
        /// Get whether friend requests are enabled
        /// </summary>
        public bool FriendRequestsEnabled { get; set; }

        /// <summary>
        /// Get the player as messenger user
        /// </summary>
        public MessengerBuddy MessengerUser => new MessengerBuddy(Player.GetHabbo());

        #endregion

        #region Constructors

        public MessengerComponent(GameClient player)
        {
            Player = player;
            FriendRequestsEnabled = true;
            LoadMessengerData(player.GetHabbo().Id);
        }

        public MessengerComponent(uint userId)
        {
            FriendRequestsEnabled = true;
            LoadMessengerData(userId);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load messenger data by given useer id
        /// </summary>
        private void LoadMessengerData(uint userId)
        {
            Friends = IonEnvironment.GetHabboHotel().GetMessenger().GetFriends(userId).Select(data => new MessengerBuddy(data.FriendData)).ToList();
            Requests = IonEnvironment.GetHabboHotel().GetMessenger().GetRequests(userId).Select(data => new MessengerBuddy(data.FriendData)).ToList();
            Categories = IonEnvironment.GetHabboHotel().GetMessenger().GetCategories(userId);
            Queue = new ConcurrentQueue<MessengerUpdate>();
        }

        /// <summary>
        /// Queue specific messenger update
        /// </summary>
        /// <param name="updateType">the update type</param>
        /// <param name="messengerUser">the messenger user</param>
        public void QueueUpdate(MessengerUpdateType updateType, MessengerBuddy messengerUser)
        {
            Queue.Enqueue(new MessengerUpdate
            {
                UpdateType = updateType,
                Friend = messengerUser
            });
        }

        /// <summary>
        /// Send status update to all friends
        /// </summary>
        public void SendStatus()
        {
            var onlineFriends = GetOnlineFriends();

            foreach (var friend in onlineFriends)
                friend.Client.GetMessenger().QueueUpdate(MessengerUpdateType.UpdateFriend, MessengerUser);

            foreach (var friend in onlineFriends)
                friend.Client.GetMessenger().ForceUpdate();
        }

        /// <summary>
        /// Forces update to own messenger
        /// </summary>
        public void ForceUpdate()
        {
            List<MessengerUpdate> messengerUpdates = Queue.Dequeue();

            if (messengerUpdates.Count > 0)
            {

            }
                //Player.Send(new UpdateMessengerComposer(Categories, messengerUpdates));
        }

        /// <summary>
        /// Remove friend by user id
        /// </summary>
        public void RemoveFriend(int id)
        {
            Friends.RemoveAll(x => x.Habbo.Id == id);
        }

        /// <summary>
        /// Remove request by user id
        /// </summary>
        public void RemoveRequest(int id)
        {
            Requests.RemoveAll(x => x.Habbo.Id == id);
        }

        /// <summary>
        /// Get the list of all online friends
        /// </summary>
        public List<MessengerBuddy> GetOnlineFriends() => 
            Friends.Where(friend => friend.IsOnline).ToList();
             
        public bool HasFriend(uint userId) => 
            Friends.Count(friend => friend.Habbo.Id == userId) > 0;

        public MessengerBuddy GetFriend(uint userId) =>
            Friends.FirstOrDefault(friend => friend.Habbo.Id == userId);

        public bool HasRequest(uint userId) =>
            Requests.Count(requester => requester.Habbo.Id == userId) > 0;

        public MessengerBuddy GetRequest(uint userId) =>
            Requests.FirstOrDefault(friend => friend.Habbo.Id == userId);

        #endregion
    }
}
