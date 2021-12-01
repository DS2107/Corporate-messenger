using Android;
using Android.Media;
using Corporate_messenger.Service;
using Plugin.AudioRecorder;
using SIPSorcery.Net;
using SIPSorcery.SIP;
using SIPSorcery.SIP.App;
using SIPSorceryMedia.Abstractions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace Corporate_messenger.ViewModels
{
    class CallClass
    {
        AudioRecorderService recorder;
        public CallClass()
        {
            recorder = new AudioRecorderService
            {
                StopRecordingOnSilence = false, //will stop recording after 2 seconds (default)
                StopRecordingAfterTimeout = false,  //stop recording after a max timeout (defined below)
              
            };
           var s = recorder.GetAudioFileStream();

            AudioFunctions.WriteWavHeader(recorder.GetAudioFileStream(), 1, (int)AudioSamplingRatesEnum.Rate8KHz, 16);
        }

        public async Task CallUserAsync()
        {
            try
            {
                if (!recorder.IsRecording)                
                    await recorder.StartRecording();                
                else               
                    await recorder.StopRecording();
                
            }
            catch (Exception ex)
            {

            }
        }

    }
}
