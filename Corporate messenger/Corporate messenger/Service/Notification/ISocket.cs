using System;
using System.Collections.Generic;
using System.Text;

namespace Corporate_messenger.Service.Notification
{
    public interface ISocket
    {
        public WebSocketSharp.WebSocket MyWebSocket { get; set; }
    }
}
