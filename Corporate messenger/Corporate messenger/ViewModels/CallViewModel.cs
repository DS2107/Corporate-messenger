using SIPSorcery.SIP;
using Corporate_messenger.Service;
using System.Windows.Input;
using Xamarin.Forms;
using System.ComponentModel;
using Corporate_messenger.Service.Notification;
using Newtonsoft.Json;
using Corporate_messenger.Models;
using Corporate_messenger.Views;
using System.Timers;
using System;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Corporate_messenger.DB;

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
      
        public CallViewModel()
        {
            
        }
       
        public async Task ClosePageAsync()
        {

            await navigate.PopToRootAsync();


        }


         public CallViewModel(INavigation nav,bool init_call)
        {
            
           
            TimeCall = "Инициализация звонка...";
            DependencyService.Get<IAudioWebSocketCall>().callView = this;
            DependencyService.Get<IAudioWebSocketCall>().InitAudioWebSocketCallAsync();
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

       

        public static  INavigation navigate;
        public bool FlagInitCall { get; set; }
        public int mins { get; set; }
        public int secs { get; set; }
        public int h { get; set; }
        private static  string timeCall { get; set; }
        Timer timer;
        private string sourceHold { get; set; }
        private string sourceMic { get; set; }
        private bool visibleButtonEnd { get; set; }
        private bool visibleButtonEndCenter { get; set; }
        private bool visibleButtonStart { get; set; }
        public Color colorBTN { get; set; }
        public bool micflag { get; set; }
        public bool holdflag { get; set; }

        /// <summary>
        /// Время звонка
        /// </summary>
        public string TimeCall
        {
            get { return timeCall; }
            set
            {
                if (timeCall != value)
                {
                    timeCall = value;
                    OnPropertyChanged("TimeCall");
                }
            }
        }
  

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
                return new Command( (object obj) => {
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
                return new Command( (object obj) => {
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
                    var MyUser = await UserDbService.GetUser();
                    if (DependencyService.Get<IAudioUDPSocketCall>().FlagRaised == true)
                    {
                      
                        DependencyService.Get<ISocket>().MyWebSocket.Send(JsonConvert.SerializeObject(new { type = "init_call", status = "400",sender_id = MyUser.Id }));
                        await navigate.PopToRootAsync();
                        DependencyService.Get<IAudioUDPSocketCall>().StopAudioUDPCall();
                      
                    }
                    else
                    {
                        TimeCall = "Вызов Завершен";
                        DependencyService.Get<ISocket>().MyWebSocket.Send(JsonConvert.SerializeObject(new { type = "init_call", status = "402", sender_id = MyUser.Id }));
                        await navigate.PopToRootAsync();
                      //  DependencyService.Get<IAudioUDPSocketCall>().StopAudioUDPCall();
                        // Флаг Для отключения музыки 
                        DependencyService.Get<IForegroundService>().Flag_AudioCalls_Init = false;

                        // Полное выключение музыки
                        DependencyService.Get<IAudio>().StopAudioFile();
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
                return new Command( (object obj) => {

                    DependencyService.Get<ISocket>().MyWebSocket.Send(JsonConvert.SerializeObject(new { type = "init_call", status = "400" }));

                    if (!FlagInitCall)
                    {
                        DependencyService.Get<IAudio>().StopAudioFile();
                        DependencyService.Get<ICloseApplication>().closeApplication();
                    }
                    
                });
            }
        }
       
      
      
        /// Ответиьт на звонок
        /// </summary>
        public ICommand StartCall
        {
            get
            {
                return new Command(async (object obj) => {
                
                    var user = await UserDbService.GetUser();
                    var vrem = DependencyService.Get<IForegroundService>().receiver_id;

                    DependencyService.Get<IAudioUDPSocketCall>().ConnectionToServer();
                    DependencyService.Get<IAudioUDPSocketCall>().GetServerIp();


                    DependencyService.Get<ISocket>().MyWebSocket.Send(JsonConvert.SerializeObject(new
                    {
                        type = "init_call",
                        sender_id = user.Id,
                        status = "200",
                        receiver_id = DependencyService.Get<IForegroundService>().receiver_id,
                        call_address = DependencyService.Get<IAudioUDPSocketCall>().GetServerIp(),
                        call_id = DependencyService.Get<IForegroundService>().call_id
                    })) ;

                    DependencyService.Get<IForegroundService>().Flag_AudioCalls_Init = false;
                   
                    DependencyService.Get<IAudio>().StopAudioFile();
                    DependencyService.Get<IAudioUDPSocketCall>().InitUDP();
                    DependencyService.Get<IAudioUDPSocketCall>().SendMessage();
                
                    DependencyService.Get<IAudioUDPSocketCall>().StartReceive();
              

                    //DependencyService.Get<IAudioWebSocketCall>().FlagRaised = true;
                    //DependencyService.Get<IAudioWebSocketCall>().StartAudioWebSocketCallAsync(ws);

                    //  ColorBTN = Color.Red;
                    VisibleButtonStart = false;
                    VisibleButtonEnd = false;
                    VisibleButtonEndCenter = true;
                  

                });
            }
        }
        public void TStart()
        {
            timer = new Timer();
            timer.Interval = 1000; // 1 sec 
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            secs += 1;
            if (secs == 60)
            {
                secs = 0;
                mins += 1;
            }
            if (mins == 60)
            {
                mins = 0;
                h += 1;
            }
            TimeCall = string.Format("{0}:{1}:{2}", h.ToString().PadLeft(2, '0'), mins.ToString().PadLeft(2, '0'), secs.ToString().PadLeft(2, '0'));

        }
        public async Task TStopAsync()
        {
          
            await navigate.PopAsync();
            
            timer.Stop();
        }

    }
}
