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
            try
            {
               // await navigate.PopAsync();
            }
            catch (Exception ex)
            {
                var b = ex;
            }
          
        }


         public CallViewModel(INavigation nav,bool init_call)
        {
            
            ws = DependencyService.Get<ISocket>().MyWebSocket;
            TimeCall = "Инициализация звонка...";
            DependencyService.Get<IAudioWebSocketCall>().callView = this;
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
                        DependencyService.Get<IAudioWebSocketCall>().StopAudioRecord();
                    }
                    else
                    {
                        SourceMic = "MicOn24.png";
                        micflag = false;
                        DependencyService.Get<IAudioWebSocketCall>().StartAudioWebSocketCallAsync(ws);
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
                    var flag = DependencyService.Get<IAudioWebSocketCall>().FlagRaised;
                    if (flag)
                    {
                        ws.Send(JsonConvert.SerializeObject(new { type = "init_call", status = "400",TimeCall }));
                        DependencyService.Get<IAudioWebSocketCall>().FlagRaised = false;
                       
                    }
                    else
                    {
                        TimeCall = "Вызов Завершен";
                        ws.Send(JsonConvert.SerializeObject(new { type = "init_call", status = "450" }));
                    }
                     
                    if (FlagInitCall)
                    {
                      await navigate.PopAsync();
                    }                                      
                    else
                    {
                        Application.Current.MainPage = new AuthorizationMainPage();
                        navigate = Application.Current.MainPage.Navigation;
                       
                        await navigate.PopToRootAsync();

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
                    ws.Send(JsonConvert.SerializeObject(new { type = "init_call", sender_id = user.Id,status ="200",receiver_id =1, DependencyService.Get<IForegroundService>().call_id}));
                    DependencyService.Get<IForegroundService>().AudioCalls_Init = false;
                   
                     DependencyService.Get<IAudio>().StopAudioFile();
                    DependencyService.Get<IAudioWebSocketCall>().FlagRaised = true;
                    DependencyService.Get<IAudioWebSocketCall>().StartAudioWebSocketCallAsync(ws);
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
