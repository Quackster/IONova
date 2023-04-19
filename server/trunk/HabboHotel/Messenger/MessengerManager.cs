using Ion.Storage.Models;
using Ion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Ion.HabboHotel.Messenger
{
    public class MessengerManager
    {
        public List<Habbo> SearchMessenger(string query, uint ignoreUserId, int searchResultLimit = 30)
        {
            using (var context = IonEnvironment.GetDatabase().GetContext())
            {
                return context.Habbo.Where(x => x.Name.ToLower().StartsWith(query.ToLower())
                    && x.Id != ignoreUserId)
                    .Take(searchResultLimit).ToList();
            }
        }


        public List<MessengerRequest> GetRequests(uint userId)
        {
            using (var context = IonEnvironment.GetDatabase().GetContext())
            {
                return context.MessengerRequest.Where(x => x.UserId == userId)
                    .Include(x => x.FriendData)
                    .ToList();
            }
        }

        public List<MessengerFriend> GetFriends(uint userId)
        {
            using (var context = IonEnvironment.GetDatabase().GetContext())
            {
                return context.MessengerFriend.Where(x => x.UserId == userId)
                    .Include(x => x.FriendData)
                    .ToList();
            }
        }

        public List<MessengerCategory> GetCategories(uint userId)
        {
            using (var context = IonEnvironment.GetDatabase().GetContext())
            {
                return context.MessengerCategory.Where(x => x.UserId == userId).ToList();
            }
        }

        public void DeleteRequests(uint userId, uint friendId)
        {
            using (var context = IonEnvironment.GetDatabase().GetContext())
            {
                context.MessengerRequest.RemoveRange(
                    context.MessengerRequest.Where(x =>
                        (x.FriendId == friendId && x.UserId == userId) ||
                        (x.FriendId == userId && x.UserId == friendId))
                    );
            }
        }

        public void DeleteAllRequests(uint userId)
        {
            using (var context = IonEnvironment.GetDatabase().GetContext())
            {
                context.MessengerRequest.RemoveRange(
                    context.MessengerRequest.Where(x =>
                        (x.FriendId == userId || x.UserId == userId))
                    );
            }
        }

        public void DeleteFriends(uint userId, uint friendId)
        {
            using (var context = IonEnvironment.GetDatabase().GetContext())
            {
                context.MessengerFriend.RemoveRange(
                    context.MessengerFriend.Where(x =>
                       (x.FriendId == friendId && x.UserId == userId) ||
                       (x.FriendId == userId && x.UserId == friendId))
                    );
            }
        }

        public void SaveRequest(MessengerRequest messengerRequestData)
        {
            using (var context = IonEnvironment.GetDatabase().GetContext())
            {
                context.MessengerRequest.Add(messengerRequestData);
                context.SaveChanges();
            }
        }

        public void SaveFriend(MessengerFriend messengerFriendData)
        {
            using (var context = IonEnvironment.GetDatabase().GetContext())
            {
                context.MessengerFriend.Add(messengerFriendData);
                context.SaveChanges();
            }
        }

        public void SaveMessage(MessengerChat messengerChatData)
        {
            using (var context = IonEnvironment.GetDatabase().GetContext())
            {
                context.MessengerChat.Add(messengerChatData);
                context.SaveChanges();
            }
        }

        public void SetReadMessages(uint userId)
        {
            using (var context = IonEnvironment.GetDatabase().GetContext())
            {
                context.MessengerChat
                    .Where(x => x.FriendId == userId && !x.IsRead)
                    .ToList()
                    .ForEach(x =>
                    {
                        x.IsRead = true;
                    });

                context.SaveChanges();
            }
        }

        public List<MessengerChat> GetUneadMessages(uint userId)
        {
            using (var context = IonEnvironment.GetDatabase().GetContext())
            {
                return context.MessengerChat.Where(x => x.FriendId == userId && !x.IsRead).ToList();
            }
        }
    }
}
