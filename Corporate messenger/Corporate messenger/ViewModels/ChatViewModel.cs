
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        async void ConnectToServerAsync()
        {
            await client.ConnectAsync(new Uri("ws://192.168.0.105:6001/app/ABCDEFG"), CancellationToken.None);
            var data = "Helo";
            var encoder = Encoding.UTF8.GetBytes(data);
            var buffer = new ArraySegment<Byte>(encoder, 0, encoder.Length);
            await client.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            //client.Abort();
        }
    }
}
