using KenshinChat.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KenshinChat.Client.Services
{
    public interface IHubService
    {
        //Events
        public event Action<ChatMessage> RecieveMessage;
        public event Action<List<User>> GetAllUsers;
        public event Action<User> UpdateUser;
        public event Action<User> AddNewUser;
        public event Action<List<ChatMessage>> ReceiveChatLog;

        Task<bool> ConnectAsync(string accessToken);
        Task SendMessage(ChatMessage message);
        Task RegisterToHub(CurrentUser user);
        Task UpdateIsTyping(bool isTyping);
    }
}
