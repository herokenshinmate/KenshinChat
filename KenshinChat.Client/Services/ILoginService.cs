using KenshinChat.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KenshinChat.Client.Services
{
    public interface ILoginService
    {
        Task<string> AttemptLogin(User user);
    }
}
