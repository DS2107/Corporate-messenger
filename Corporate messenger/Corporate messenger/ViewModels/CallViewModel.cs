
using System.Net;
using System;
using SIPSorcery.SIP;
using SIPSorcery.SIP.App;
using SIPSorcery.Net;
using SIPSorcery.Media;
using Android.Media;
using SIPSorceryMedia.Abstractions;
using Corporate_messenger.Service;


namespace Corporate_messenger.ViewModels
{
   

    public class CallViewModel
    {
        string USERNAME = "1001";
        string PASSWORD = "1234";
        string DOMAIN = "192.168.0.105";
        int EXPIRY = 120;
        

        SIPTransport sipTransport = new SIPTransport();
     
        public CallViewModel()
        {


        
           // myCall();
        }
        RTPSession rtpSession;


       
       
        public  async System.Threading.Tasks.Task myCall()
        {
            SIPRegistrationUserAgent regUserAgent = new SIPRegistrationUserAgent(sipTransport, USERNAME, PASSWORD, DOMAIN, EXPIRY);
            regUserAgent.RegistrationFailed += Failed;
            regUserAgent.RegistrationTemporaryFailure += Failure;
            regUserAgent.RegistrationRemoved += RegRemove;
            regUserAgent.RegistrationSuccessful += Success;
            regUserAgent.Start();
            
           
          string hostName = Dns.GetHostName(); // Retrive the Name of HOST  

          //string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
          System.Net.IPAddress ipaddress = System.Net.IPAddress.Parse("192.168.0.100");
          var userAgent = new SIPUserAgent(sipTransport, null);
              rtpSession = new RTPSession(false, true,false,null,0);
            
          
            SIPUserAgent ua = new SIPUserAgent();
           
            bool callResult = await userAgent.Call("1002@192.168.0.105", "1001", "1234", rtpSession);





            SIPUDPChannel udpChannel = new SIPUDPChannel(new IPEndPoint(IPAddress.Any, 0));
          sipTransport.AddSIPChannel(udpChannel);
          SIPClientUserAgent uac = new SIPClientUserAgent(sipTransport);
           
           // _ = rtpSession.Start();

            SIPCallDescriptor callDescriptor = new SIPCallDescriptor("1001", "1234", "sip:1002@192.168.0.105", "<sip:1001@192.168.0.105>", null, null, null, null, SIPCallDirection.Out, "application/sdp", null, null);
             //uac.Call(callDescriptor);
           

          
           
            uac.CallAnswered += Uac_CallAnswered;
         
           
        }

        private void Uac_CallAnswered(ISIPClientUserAgent uac, SIPResponse sipResponse)
        {
          
            rtpSession.Start();
            MediaRecorder ClassMedia = new MediaRecorder();

            ClassMedia.SetAudioSource(AudioSource.VoiceCall);
            ClassMedia.SetOutputFormat(OutputFormat.AmrNb);
            ClassMedia.SetAudioEncoder(Android.Media.AudioEncoder.AmrNb);

            ClassMedia.Prepare();
            ClassMedia.Start();
        }

      

        private void Success(SIPURI obj)
        {
            throw new NotImplementedException();
        }

        private void RegRemove(SIPURI obj)
        {
            throw new NotImplementedException();
        }

        private void Failure(SIPURI arg1, string arg2)
        {
            throw new NotImplementedException();
        }

        private void Failed(SIPURI arg1, string arg2)
        {
            throw new NotImplementedException();
        }
    }
}
