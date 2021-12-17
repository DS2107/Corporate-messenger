
using System.Net;
using System;
using SIPSorcery.SIP;
using SIPSorcery.SIP.App;
using SIPSorcery.Net;
using SIPSorcery.Media;
using Android.Media;
using SIPSorceryMedia.Abstractions;
using Corporate_messenger.Service;
using System.Windows.Input;
using Xamarin.Forms;
using System.ComponentModel;
using Corporate_messenger.Service.Notification;
using Newtonsoft.Json;
using Corporate_messenger.Models;
using Corporate_messenger.Views;
using System.Threading.Tasks;

namespace Corporate_messenger.ViewModels
{


    public class CallViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        string USERNAME = "1001";
        string PASSWORD = "1234";
        string DOMAIN = "192.168.0.105";
        int EXPIRY = 120;


        SIPTransport sipTransport = new SIPTransport();
        INavigation navigate;
        public bool FlagInitCall { get; set; }
        public CallViewModel(INavigation nav,bool init_call)
        {
            DependencyService.Get<IAudioWebSocketCall>().InitAudioWebSocketCall(user.Id);
            FlagInitCall = init_call;
            navigate = nav;
            SourceHold = "HoldCall.png";
            SourceMic = "MicOn24.png";
            ColorBTN = Color.FromHex("#5BD782");
            if (FlagInitCall)
            {
                VisibleButtonEndCenter = true;
                VisibleButtonStart = false;
                VisibleButtonEnd = false;
             
            }
                
            else
            {
                VisibleButtonEndCenter = false;
                VisibleButtonStart = true;
                VisibleButtonEnd = true;
            }
                

            // myCall();
        }

        private string sourceHold { get; set; }
        private string sourceMic { get; set; }
        private bool visibleButtonEnd { get; set; }
        private bool visibleButtonEndCenter { get; set; }
        private bool visibleButtonStart { get; set; }
        public Color colorBTN { get; set; }
        public bool micflag { get; set; }
        public bool holdflag { get; set; }
        /// <summary>
        /// Видимость  кнопки вызов
        /// </summary>
        public Color ColorBTN
        {
            get { return colorBTN; }
            set
            {
                if (colorBTN != value)
                {
                    colorBTN = value;
                    OnPropertyChanged("ColorBTN");
                }
            }
        }
        /// <summary>
        /// Видимость  кнопки вызов
        /// </summary>
        public bool VisibleButtonEnd
        {
            get { return visibleButtonEnd; }
            set
            {
                if (visibleButtonEnd != value)
                {
                    visibleButtonEnd = value;
                    OnPropertyChanged("VisibleButtonEnd");
                }
            }
        }
        /// <summary>
        /// Видимость  кнопки вызов
        /// </summary>
        public bool VisibleButtonEndCenter
        {
            get { return visibleButtonEndCenter; }
            set
            {
                if (visibleButtonEndCenter != value)
                {
                    visibleButtonEndCenter = value;
                    OnPropertyChanged("VisibleButtonEndCenter");
                }
            }
        }
        /// <summary>
        /// Видимость  кнопки вызов
        /// </summary>
        public bool VisibleButtonStart
        {
            get { return visibleButtonStart; }
            set
            {
                if (visibleButtonStart != value)
                {
                    visibleButtonStart = value;
                    OnPropertyChanged("VisibleButtonStart");
                }
            }
        }
        /// <summary>
        /// Картинка паузы
        /// </summary>
        public string SourceHold
        {
            get { return sourceHold; }
            set
            {
                if (sourceHold != value)
                {
                    sourceHold = value;
                    OnPropertyChanged("SourceHold");
                }
            }
        }
        /// <summary>
        /// Картинка Микрфона
        /// </summary>
        public string SourceMic
        {
            get { return sourceMic; }
            set
            {
                if (sourceMic != value)
                {
                    sourceMic = value;
                    OnPropertyChanged("SourceMic");
                }
            }
        }
        /// <summary>
        /// Ставит на паузу 
        /// </summary>
        public ICommand Hold
        {
            get
            {
                return new Command(async (object obj) => {
                    if (holdflag== false)
                    {

                        SourceHold = "PlayHold.png";
                        holdflag = true;
                    }
                    else
                    {
                        SourceHold = "HoldCall.png";
                        holdflag = false;
                    }
                });
            }
        }
        /// <summary>
        /// выключает микрофон
        /// </summary>
        public ICommand MicSetting
        {
            get
            {
                return new Command(async (object obj) => {
                    if (micflag == false)
                    {
                        SourceMic = "MicOff24.png";
                        micflag = true;
                    }
                    else
                    {
                        SourceMic = "MicOn24.png";
                        micflag = false;
                    }
                });
            }
        }
        Application application = new Application();
        /// <summary>
        /// Завершить звонок
        /// </summary>
        public ICommand EndCall
        {
            get
            {
                return new Command(async (object obj) => {
                    ws = DependencyService.Get<ISocket>().MyWebSocket;
                    ws.Send(JsonConvert.SerializeObject(new { type="init_call", status = "400"}));
                   
                    if (FlagInitCall)
                    {
                        navigate.PopAsync();
                      
                    }                                      
                    else
                    {
                        application.MainPage = new AuthorizationMainPage();
                        await Shell.Current.GoToAsync("//chats_list", true);

                    }
                       
                   
                });
            }
        }

        /// <summary>
        /// Завершить звонок
        /// </summary>
        public ICommand LeftEndCall
        {
            get
            {
                return new Command(async (object obj) => {
                    ws = DependencyService.Get<ISocket>().MyWebSocket;
                    ws.Send(JsonConvert.SerializeObject(new { type = "init_call", status = "400" }));

                    if (!FlagInitCall)
                    {
                        DependencyService.Get<IAudio>().StopAudioFile();
                        DependencyService.Get<ICloseApplication>().closeApplication();
                    }
                    
                });
            }
        }
        SpecialDataModel user = new SpecialDataModel();
        WebSocketSharp.WebSocket ws;
        
        /// Ответиьт на звонок
        /// </summary>
        public ICommand StartCall
        {
            get
            {
                return new Command(async (object obj) => {
                    ws = DependencyService.Get<ISocket>().MyWebSocket;
                    ws.Send(JsonConvert.SerializeObject(new { type = "init_call", sender_id = user.Id,status ="200",receiver_id =1  }));
                    DependencyService.Get<IForegroundService>().AudioCalls_Init = false;
                    DependencyService.Get<IAudio>().StopAudioFile();                   
                    DependencyService.Get<IAudioWebSocketCall>().StartAudioWebSocketCallAsync(ws);
                    //  ColorBTN = Color.Red;
                    VisibleButtonStart = false;
                    VisibleButtonEnd = false;
                    VisibleButtonEndCenter = true;
                  

                });
            }
        }




        RTPSession rtpSession;




        public async System.Threading.Tasks.Task myCall()
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
            rtpSession = new RTPSession(false, true, false, null, 0);
            //VoIPMediaSession s = new VoIPMediaSession();

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
