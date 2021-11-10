using Android;
using Android.Media;
using Corporate_messenger.Service;

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
        /*private Logger logger;
        private static SipClient client;
        private bool CallFlag = false; 
        public CallClass()
        {
           
        }

        public void LessPort()
        {
            client = new SipClient("192.168.0.105", "1002", "1234");

            //create logger
            logger = new Logger();
            logger.WriteLog += new WriteLogEventHandler(OnWriteLog);
            client.Logger = logger;

            client.ReceiveRequest += new ReceiveRequestEventHandler(OnReceiveRequest);
            client.ReceiveResponse += new ReceiveResponseEventHandler(OnReceiveResponse);

            //IPHostEntry localhost = Dns.Resolve("mycomputer"); 
            //client.LocalIPEndPoint = new IPEndPoint(localhost.AddressList[0],5060);
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST  

            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            System.Net.IPAddress localAddress = System.Net.IPAddress.Parse(myIP);
            client.LocalIPEndPoint = new System.Net.IPEndPoint(localAddress, 5060);

            client.Connect();
            client.Register("sip:192.168.0.105", "sip:1002@192.168.0.105", "sip:1002@" + client.LocalIPEndPoint.ToString());


        }
        public void Call()
        {
            CallFlag = true;
            client = new SipClient("192.168.0.105", "1001", "1234");

            logger = new Logger();
            logger.WriteLog += new WriteLogEventHandler(OnWriteLog);
            client.Logger = logger;

            client.ReceiveRequest += new ReceiveRequestEventHandler(OnReceiveRequest);
            client.ReceiveResponse += new ReceiveResponseEventHandler(OnReceiveResponse);

            client.Connect();

            SessionDescription session = new SessionDescription();
            session.Version = 0;
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
         
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            Owner owner = new Owner();
            owner.Username = "danil";
            owner.SessionID = 16264;
            owner.Version = 18299;
            owner.Address = myIP;

            session.Owner = owner;
            session.Name = "SIP Call";

            Connection connection = new Connection();
            connection.Address = myIP;

            session.Connection = connection;

            Time time = new Time(0, 0);
            session.Time.Add(time);

            Media media1 = new Media();
            media1.Type = "audio";
            media1.Port = 25282;
            media1.TransportProtocol = "RTP/AVP";
            media1.MediaFormats.Add("0");
            media1.MediaFormats.Add("101");

            media1.Attributes.Add("rtpmap", "0 pcmu/8000");
            media1.Attributes.Add("rtpmap", "101 telephone-event/8000");
            media1.Attributes.Add("fmtp", "101 0-11");

            session.Media.Add(media1);

           
            RequestResponse inviteRequestResponse = client.Invite("sip:1001@192.168.0.105", "sip:1002@192.168.0.105", "sip:1001@" + client.LocalIPEndPoint.ToString(), null, null);
            SessionDescription bobSession = inviteRequestResponse.Response.SessionDescription;
            client.Ack(inviteRequestResponse);

         
        }


        bool inviteaccept = false;
        private  void OnReceiveRequest(object sender, RequestEventArgs e)
        {
            if (CallFlag == false)
            {
                if(inviteaccept == false)
                {
                    try
                    {
                        DependencyService.Get<IAudio>().PlayAudioFile("zvonok.mp3");
                        if (e.Request.Method == SipMethod.Invite)
                        {
                            inviteaccept = true;
                            OK ok = new OK();
                            ok.SessionDescription = GenerateSessionDescription();
                            client.SendResponse(ok, e.Request);
                            DependencyService.Get<IAudio>().PlayStop();
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
              
            }
           

           
                client.AcceptRequest(e.Request);
            

        }

        private static void OnReceiveResponse(object sender, ResponseEventArgs e)            
        {

        }
        private static SessionDescription GenerateSessionDescription()
        {
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST  

            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            SessionDescription session = new SessionDescription();
            session.Version = 0;

            Owner owner = new Owner();
            owner.Username = "Bob";
            owner.SessionID = 16264;
            owner.Version = 18299;
            owner.Address = myIP;

            session.Owner = owner;
            session.Name = "SIP Call";

            Connection connection = new Connection();
            connection.Address = myIP;

            session.Connection = connection;

            Time time = new Time(0, 0);
            session.Time.Add(time);

            Media media1 = new Media();
            media1.Type = "audio";
            media1.Port = 25282;
            media1.TransportProtocol = "RTP/AVP";
            media1.MediaFormats.Add("0");
            media1.MediaFormats.Add("101");

            media1.Attributes.Add("rtpmap", "0 pcmu/8000");
            media1.Attributes.Add("rtpmap", "101 telephone-event/8000");
            media1.Attributes.Add("fmtp", "101 0-11");

            session.Media.Add(media1);

            return session;
        }
        private static void OnWriteLog(object sender, WriteLogEventArgs e)
        {
            Console.Write(e.Log);
        }*/
    }
}
