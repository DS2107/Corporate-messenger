using System;
using System.Collections.Generic;
using System.Text;

namespace Corporate_messenger.Service
{
   
    public interface IAudio
    {
        void PlayAudioFile(string fileName);
        void SendMessageAudioCommand();
        void StopSendMessageAudioCommandAsync();
        void StopAudioFile();
        public double GetInfo();
        public double GetPosition();

        public void Resume();
    }
}
