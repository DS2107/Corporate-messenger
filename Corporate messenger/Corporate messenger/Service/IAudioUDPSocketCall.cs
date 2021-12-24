using Corporate_messenger.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Corporate_messenger.Service
{
    public interface IAudioUDPSocketCall
    {
        void InitUDP(int usr_id, int rec_id);
        public Task StartAudioUDPCallAsync();

        public void StopAudioUDPCall();

        public CallViewModel callView { get; set; }
     
    }
}
