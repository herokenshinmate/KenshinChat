using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KenshinChat.Server.Models
{
    public class ChatUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public bool IsOnline { get; set; }
        public bool IsTyping { get; set; }
    }
}
