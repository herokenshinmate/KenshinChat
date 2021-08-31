using KenshinChat.Client.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace KenshinChat.Client.Services
{
    public class LoginService : ILoginService
    {
        private readonly string URL = "http://localhost:27738/Login/Login";

        public async Task<string> AttemptLogin(User user)
        {
            using var client = new HttpClient();
            var json = JsonConvert.SerializeObject(user);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(URL, data);

            string result = response.Content.ReadAsStringAsync().Result;

            Debug.Write("Created client: Result = " + result);

            return result;
        }
    }
}
