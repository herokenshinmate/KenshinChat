using KenshinChat.Server.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KenshinChat.Server.Hubs
{
    [Authorize(JwtBearerDefaults.AuthenticationScheme)]
    public class DefaultHub : Hub
    {
        private static ConcurrentDictionary<string, ChatUser> ChatClients = new ConcurrentDictionary<string, ChatUser>();
        private static ConcurrentDictionary<ChatUser, ChatMessage> Chat = new ConcurrentDictionary<ChatUser, ChatMessage>();
        private readonly ApiDbContext _dbContext;
        
        public DefaultHub()
        {
            _dbContext = new ApiDbContext();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            ChatUser user = ChatClients.FirstOrDefault(u => u.Key == Context.ConnectionId).Value;
            if (user != null)
            {
                ChatUser updated = user;
                updated.IsOnline = false;
                ChatClients.TryUpdate(Context.ConnectionId, updated, user);

                await Clients.AllExcept(Context.ConnectionId).SendAsync("UpdateUser", updated);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task RegisterToHub(User user)
        {
            ChatUser newChatUser = new ChatUser
            {
                UserId = user.UserId,
                UserName = user.Username,
                IsOnline = true
            };

            //If the user exists on an old connection, then remove it
            var kvp = ChatClients.FirstOrDefault(k => k.Value.UserId == newChatUser.UserId);
            if(kvp.Key != null)
                ChatClients.TryRemove(kvp);

            ChatClients.TryAdd(Context.ConnectionId, newChatUser);

            //Send the client the list of users
            await Clients.Caller.SendAsync("GetAllUsers", ChatClients.Values.ToList());

            //Send all the other clients the new client
            await Clients.AllExcept(Context.ConnectionId).SendAsync("AddNewUser", newChatUser);

            //Send the client the chat log
            await Clients.Caller.SendAsync("ReceiveChatLog", Chat.Values.ToList());
        }

        public async Task SendMessage(ChatMessage message)
        {
            ChatUser messenger = new ChatUser
            {
                UserId = message.AuthorId,
                UserName = message.Author
            };

            Chat.TryAdd(messenger, message);

            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task SetIsTyping(bool isTyping)
        {
            ChatUser oldUser, updatedUser;
            ChatClients.TryGetValue(Context.ConnectionId, out oldUser);
            if (oldUser != null)
            {
                updatedUser = oldUser;
                updatedUser.IsTyping = isTyping;

                ChatClients.TryUpdate(Context.ConnectionId, updatedUser, oldUser);

                await Clients.AllExcept(Context.ConnectionId).SendAsync("UpdateUser", updatedUser);
            }
        }
    }
}
