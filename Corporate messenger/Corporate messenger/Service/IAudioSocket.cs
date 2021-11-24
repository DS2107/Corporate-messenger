using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Corporate_messenger.Service
{
    public interface IAudioSocket
    {
        public void Init(int receiverId, int sender_id);





        public void FileCreate();


        public void Start(WebSocketSharp.WebSocket ws);
        Task Start2(WebSocketSharp.WebSocket ws);
        void PlayVoiceChat(byte[] audio_message);
    }
}
