﻿using KenshinChat.Client.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.Extensions.Logging.Console;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Logging;

namespace KenshinChat.Client.Services
{
    public class HubService : IHubService
    {
        private HubConnection connection;

        //Events
        public event Action<ChatMessage> RecieveMessage;
        public event Action<List<User>> UsersUpdate;
        public event Action<List<ChatMessage>> ReceiveChatLog;

        public async Task<bool> ConnectAsync(string accessToken)
        {
            try
            {
                connection = new HubConnectionBuilder()
                    .WithUrl("http://localhost:47252/kenshinhub", options =>
                    {
                        options.AccessTokenProvider = () => Task.FromResult(accessToken);
                        
                        options.Headers.Add("Authorization", string.Format("Bearer {0}", accessToken));
                    })
                    .ConfigureLogging(logging =>
                    {
                        logging.SetMinimumLevel(LogLevel.Debug);
                        logging.AddConsole();
                    })
                    .Build();

                //Add our callback for messages
                connection.On<ChatMessage>("ReceiveMessage", (cm) => RecieveMessage?.Invoke(cm));
                connection.On<List<User>>("UsersUpdate", (l) => UsersUpdate?.Invoke(l));
                connection.On<List<ChatMessage>>("ReceiveChatLog", (d) => ReceiveChatLog?.Invoke(d));

                await connection.StartAsync();

                return true;
            }
            catch (HttpRequestException ex)
            {
                return false;
            }
        }

        public async Task SendMessage(ChatMessage message)
        {
            await connection.InvokeAsync("SendMessage", message);
        }

        public async Task RegisterToHub(CurrentUser user)
        {
            await connection.InvokeAsync("RegisterToHub", new User { UserId = user.UserId, Username = user.Username, IsOnline = true });
        }
    }
}
