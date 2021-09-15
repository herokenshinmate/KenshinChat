using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KenshinChat.Client.Models
{
    public class ChatMessage
    {
        public string Message { get; set; }
        public string Author { get; set; }
        public int AuthorId { get; set; }
        public DateTime Time { get; set; }
        public bool IsOriginNative { get; set; } //Set when locally acquired from hub
    }
}
