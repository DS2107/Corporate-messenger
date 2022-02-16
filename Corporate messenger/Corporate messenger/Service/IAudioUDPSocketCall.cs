using Corporate_messenger.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Corporate_messenger.Service
{
    public interface IAudioUDPSocketCall
    {
        public void ConnectionToServer();
        public string GetServerIp();
        public void StartReceive();
        public void SendMessage();
        public void ReceiveMessage();
        void InitUDP();
       

        public void StopAudioUDPCall();
        public bool FlagRaised { get; set; }
        public CallViewModel callView { get; set; }
     
    }
}
