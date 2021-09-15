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
        public event Action<List<User>> UsersUpdate;
        public event Action<List<ChatMessage>> ReceiveChatLog;

        Task<bool> ConnectAsync(string accessToken);
        Task SendMessage(ChatMessage message);
        Task RegisterToHub(CurrentUser user);
    }
}
