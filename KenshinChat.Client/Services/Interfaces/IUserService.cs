using KenshinChat.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KenshinChat.Client.Services
{
    public interface IUserService
    {
        Task<LoginResponse> AttemptLogin(UserRequest user);
        Task<LoginResponse> AttemptRegister(UserRequest user);
        Task<byte[]> GetProfilePicture(string accessToken, int userId);
    }
}
