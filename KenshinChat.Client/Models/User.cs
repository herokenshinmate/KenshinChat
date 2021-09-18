using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KenshinChat.Client.Models
{
    public class User
    {
        public int UserId { get; set; }
        public byte[] ProfilePicture { get; set; }
        public string Username { get; set; }
        public bool IsOnline { get; set; }
        public bool IsTyping { get; set; }
    }
}
