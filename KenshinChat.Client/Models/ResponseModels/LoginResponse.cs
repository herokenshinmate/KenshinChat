using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KenshinChat.Client.Models
{
    public class LoginResponse
    {
        public int UserId { get; set; }
        public byte[] ProfilePicture { get; set; }
        public string Username { get; set; }
        public string AccessToken { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
