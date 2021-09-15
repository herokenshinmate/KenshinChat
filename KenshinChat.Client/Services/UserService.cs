using KenshinChat.Client.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace KenshinChat.Client.Services
{
    public class UserService : IUserService
    {
        private readonly string URL = "http://localhost:47252/api/";
        private readonly string loginEndPoint = "User/login";
        private readonly string registerEndPoint = "User/register";

        public async Task<LoginResponse> AttemptLogin(UserRequest user)
        {
            using var client = new HttpClient();
            var json = JsonConvert.SerializeObject(user);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(URL + loginEndPoint, data);
            LoginResponse loginResponse;

            if(response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;

                loginResponse = JsonConvert.DeserializeObject<LoginResponse>(result);

                loginResponse.Success = true;

                Debug.Write("LoginSucces: Result = " + result);
            }
            else
            {
                loginResponse = new LoginResponse();
                loginResponse.Success = false;

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    loginResponse.ErrorMessage = "Username and or password are incorrect.";
                else
                    loginResponse.ErrorMessage = "Server is uncontactable.";
            }

            return loginResponse;
        }

        public async Task<LoginResponse> AttemptRegister(UserRequest user)
        {
            using HttpClient client = new HttpClient();
            string json = JsonConvert.SerializeObject(user);
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(URL + registerEndPoint, data);
            LoginResponse loginResponse;         

            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;

                loginResponse = JsonConvert.DeserializeObject<LoginResponse>(result);

                loginResponse.Success = true;

                Debug.Write("RegisterSucces: Result = " + result);
            }
            else
            {
                loginResponse = new LoginResponse();
                loginResponse.Success = false;

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    loginResponse.ErrorMessage = "User already exists with this Username";
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                    loginResponse.ErrorMessage = "Issue with the username or password";
                else
                    loginResponse.ErrorMessage = "Server is uncontactable.";
            }

            return loginResponse;
        }
    }
}
