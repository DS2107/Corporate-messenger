
using Corporate_messenger.Models;
using Corporate_messenger.Models.Chat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



namespace Corporate_messenger.ViewModels
{
    class ChatViewModel
    {
        ClientWebSocket client = new ClientWebSocket();
        public ChatViewModel()
        {
            ConnectToServerAsync();
        }
        SpecialDataModel user = new SpecialDataModel();
        async void ConnectToServerAsync()
        {
            await client.ConnectAsync(new Uri("ws://192.168.0.105:6001/app/ABCDEFG"), CancellationToken.None);
            var data = "Helo";
            var encoder = Encoding.UTF8.GetBytes(data);
            var buffer = new ArraySegment<Byte>(encoder, 0, encoder.Length);
            await client.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            //client.Abort();
            ChatModel chat_to = new ChatModel();
            chat_to.Sender_id = user.Id;
            chat_to.Message = "ss";
            chat_to.Chat_room_id = 1;
            // Перед отправкой , превращаем все в json
            string jsonLog = JsonConvert.SerializeObject(chat_to);

            // Устанавливаем соеденение 
            HttpClient client2 = new HttpClient();

            // Тип данных который мы принимаем от сервера 
            var contentType = "application/json";

            // Тип Запроса
            var httpMethod = HttpMethod.Post;
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri("http://192.168.0.105:8098/api/chat/1"),
                Method = httpMethod,
                Content = new StringContent(jsonLog, System.Text.Encoding.UTF8, contentType)
            };
            // Отправка заголовка
            request.Headers.Add("Authorization", "Bearer " + user.Token);
            // Отправка данных авторизации
            var httpResponse = await client2.SendAsync(request);

            // Ответ от сервера 
            var contenJSON = await httpResponse.Content.ReadAsStringAsync();
        }
    }
}
