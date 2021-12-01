
using Corporate_messenger.Models;
using Corporate_messenger.Models.Chat;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using WebSocketSharp;
using System.IO;
using Android.Media;
using Xamarin.Essentials;
using Corporate_messenger.Service;

namespace Corporate_messenger.ViewModels
{
    class MyAudio
    {
        [JsonProperty("voice_audio")]
        public byte[] audio { get; set; }
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("sender_id")]
        public int sendr_id { get; set; }

        [JsonProperty("receiverId")]
        public int receiverId { get; set; }

    }
    class ChatViewModel: INotifyPropertyChanged
    {
        static string addressWS = "ws://192.168.0.105:6001";
        /// <summary>
        /// Клиент для связи с сокетом
        /// </summary>
        ClientWebSocket client = new ClientWebSocket();

        /// <summary>
        /// Модель данных чат
        /// </summary>
       public ChatModel chat = new ChatModel();

         //Сокет 
        public  WebSocketSharp.WebSocket ws = new WebSocketSharp.WebSocket(addressWS);

        // Модель юзера
        public SpecialDataModel user = new SpecialDataModel();
      

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
      

        private ObservableCollection<ChatModel> messageList = new ObservableCollection<ChatModel>();
        private ObservableCollection<ChatModel> BufferList;
        private ObservableCollection<ChatModel> lastMessage = new ObservableCollection<ChatModel>();
        /// <summary>
        /// Список друзей
        /// </summary>
        public ObservableCollection<ChatModel> MessageList
        {
            get { return messageList; }
            set
            {
                if (messageList != value)
                {
                    messageList = value;
                    OnPropertyChanged("MessageList");
                }
            }
        }

      




        public ObservableCollection<ChatModel> LastMessage
        {
            get { return lastMessage; }
            set
            {
                if (lastMessage != value)
                {
                    lastMessage = value;
                    OnPropertyChanged("LastMessage");
                }
            }
        }

        


        /// <summary>
        /// Изменение коллекции
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void MessageList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add: // если добавление
                    ChatModel newMesaage = args.NewItems[0] as ChatModel;

                    break;
                case NotifyCollectionChangedAction.Remove: // если удаление
                    ChatModel oldMesaage = args.OldItems[0] as ChatModel;

                    break;
                case NotifyCollectionChangedAction.Replace: // если замена
                    ChatModel replacedMesaage = args.OldItems[0] as ChatModel;
                    ChatModel replacingMesaage = args.NewItems[0] as ChatModel;

                    break;
            }
        }

        
        private string input_message { get; set; }
        /// <summary>
        /// Сообщение пользователя
        /// </summary>
        public string Input_message
        {
            get { return input_message; }
            set
            {
                if (input_message != value)
                {
                    input_message = value;
                    OnPropertyChanged("Input_message");
                }
            }
        }


        /// <summary>
        /// Конструктор 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        public ChatViewModel(int id, string title)
        {
            chat.Chat_room_id = id;
            chat.Sender_id = user.Id;
            chat.SourceImage = "play.png";
            // Регестрирую команду для кнопки 
            SendMessage = new Command(SendMessageCommand);
            GoBack = new Command(GoBackCommand);
         
           
            MessageList.CollectionChanged += MessageList_CollectionChanged;
           
           _ = SendToken_GetChatsAsync();
         
         

        }


        
        private void WsOnMEssage(object sender, MessageEventArgs e)
        {
           var message = JsonConvert.DeserializeObject(e.Data).ToString();
            message = message.Substring(13,4);
           if(message == "call")
            {
                var myAudio = JsonConvert.DeserializeObject<MyAudio>(e.Data);
                DependencyService.Get<IAudioWebSocketCall>().ListenerWebSocketCall(myAudio.audio);

            }

             else
             {
                 ChatModel new_message = JsonConvert.DeserializeObject<ChatModel>(e.Data);
                 if (new_message.Audio == null)
                 {
                     new_message.MaximumSlider = 1;
                     new_message.IsAuidoVisible = false;
                     new_message.IsMessageVisible = true;
                     new_message.SourceImage = "play.png";
                     new_message.ValueSlider = 1;
                     try
                     {
                         LastMessage.Add(new_message);
                     }
                     catch (Exception ex)
                     {

                     }

                 }
                 else
                 {
                     new_message.ValueSlider = 1;
                     new_message.SourceImage = "play.png";
                     new_message.IsAuidoVisible = true;
                     new_message.IsMessageVisible = false;
                     new_message.MaximumSlider = 1;

                     LastMessage.Add(new_message);
                 }

                 if (new_message.Audio != null)
                 {
                     DependencyService.Get<IFileService>().SaveFile(new_message.Audio);
                 }
                 MessagingCenter.Send<ChatViewModel>(this, "Scrol"); 
            }


        }




        /// <summary>
        /// Команда для кнопки отправки сообщения
        /// </summary>
        public ICommand SendMessage { get; set; }
        public void SendMessageCommand(object obj)
        {
           
            if (Input_message != null)
            {
                byte[] audio = null;
                SendMyMessage(audio);
               
             
             
               
            }
        }

        /// <summary>
        /// Отправка сообщения
        /// </summary>
        /// <returns></returns>
        public void SendMyMessage(byte[] audio)
        {
           

            
            var new_message = new ChatModel { Chat_room_id = chat.Chat_room_id, Sender_id = chat.Sender_id, Message = Input_message, Receiver_id = user.receiverId, TypeMessage = "message",Audio=audio };
            var message = JsonConvert.SerializeObject(new_message);
            ws.Send(message);
            Input_message = "";
            

        }
       
        public ICommand GoBack { get; set; }
        private void GoBackCommand(object obj)
        {
           _ = BaskShellPageAsync();
        }
        private async Task BaskShellPageAsync()
        {
            ws.CloseAsync();
            DependencyService.Get<IAudioWebSocketCall>().StopAudioWebSocketCall();
            await Shell.Current.GoToAsync("//chats_list", true);
            ChatListViewModel chatList = new ChatListViewModel();
        }

    
        private bool isRefreshing = false;
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set
            {
                if (isRefreshing != value)
                {
                    isRefreshing = value;
                    OnPropertyChanged("IsRefreshing");
                }
            }
        }
        bool updateFlag = false;
        public ICommand UpdateList
        {

            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;
                    updateFlag = true;
                    LastMessageAdd(LastElement);
                    updateFlag = false;
                    IsRefreshing = false;
                });

            }
        }
        bool PlayStopStart = true;
        ChatModel PLayItem;
        public ICommand Test
        {

            get
            {
                return new Command(async (object obj) =>
                {
                    if (obj is ChatModel item)
                    {
                        if (PLayItem != item && PLayItem != null)
                        {
                            PLayItem.IsEnableSlider = false;
                            Stop(PLayItem);
                            Play(item);

                        }
                        else
                        {
                            if (PlayStopStart)
                            {
                                Play(item);
                            }
                            else
                            {

                                Stop(item);
                            }
                        }
                      

                    }
                  
                    
                });

            }
        }

        private void Play(ChatModel item)
        {
          
            string file = DependencyService.Get<IFileService>().SaveFile(item.Audio);
            if (File.Exists(file))
            {
                DependencyService.Get<IAudio>().PlayAudioFile(file);
                item.MaximumSlider = DependencyService.Get<IAudio>().GetFullTimeAudio();
                item.IsEnableSlider = true;
                item.SourceImage = "stop.png";

                int count = 0;
                Device.StartTimer(new TimeSpan(0, 0, 0,1), () =>
                {
                  


                        if (item.MaximumSlider != item.ValueSlider)
                        {
                            item.ValueSlider = DependencyService.Get<IAudio>().GetPositionAudio();
                            var s = DependencyService.Get<IAudio>().GetPositionAudio();
                            return true; // runs again, or false to stop
                        }
                        else
                        {
                            item.SourceImage = "Play.png";
                            PlayStopStart = true;
                            item.ValueSlider = 0;
                            return false;

                        }
                    
                  
                });
               
            }
            PlayStopStart = false;
            PLayItem = item;
        }
        private void Stop(ChatModel item)
        {
            DependencyService.Get<IAudio>().StopAudioFile();
            item.SourceImage = "play.png";
            PlayStopStart = true;
        }

        async Task SendToken_GetChatsAsync()
        {
            
            // Устанавливаем соеденение 
            HttpClient client = new HttpClient();


            // Тип Запроса
            var httpMethod = HttpMethod.Get;
            var address = DependencyService.Get<IFileService>().CreateFile() + "/api/chat/" + chat.Chat_room_id + "/" + user.Id + "/dialog";

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(address),
                Method = httpMethod,

            };
            user.Input_chat = chat.Chat_room_id;
            // Отправка заголовка
            request.Headers.Add("Authorization", "Bearer " + user.Token);

            // Отправка данных 
            var httpResponse = await client.SendAsync(request);

            // Ответ от сервера 
            var contenJSON = await httpResponse.Content.ReadAsStringAsync();

            //****** РАСШИФРОВКА_ОТВЕТА ******
            JObject contentJobjects = JObject.Parse(contenJSON);
           
            foreach (var KeyJobject in contentJobjects)
            {
                if (KeyJobject.Key == "dialog")
                {
                    var ValueJobject = JsonConvert.SerializeObject(KeyJobject.Value);
                   
                    
                    MessageList = JsonConvert.DeserializeObject<ObservableCollection<ChatModel>>(ValueJobject);
                  
                        foreach (var item in MessageList)
                        {
                            if (item.Audio != null)
                            {
                                item.ValueSlider = 1;
                                item.MaximumSlider = 1;
                                item.IsMessageVisible = false;
                                item.IsAuidoVisible = true;
                                item.SourceImage = "play.png";
                            

                            }
                            else
                            {
                            item.ValueSlider = 1;
                            item.MaximumSlider = 1;
                            item.IsMessageVisible = true;
                                item.IsAuidoVisible = false;
                            }
                        
                    }
                   
                   
                }
                if (KeyJobject.Key == "receiver_id")
                {
                    var ValueJobject = JsonConvert.SerializeObject(KeyJobject.Value);
                   var rec  = JsonConvert.DeserializeObject(ValueJobject);
                    string recc = rec.ToString();
                    string[] words = recc.Split("\n\t% ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    user.receiverId = Int32.Parse(words[2]);
                }
            }
            if (MessageList.Count > 20)
                LastMessageAdd(MessageList.Count - 1);
            else
                LastMessage = MessageList;

            ConnectToServerAsync();
            /* // Добавить в базу последние элементы
             foreach (var item in MessageList)
             {
                 App.Database.SaveItem(item);
             }*/

        }

        public int LastElement { get; set; }
        void LastMessageAdd(int count)
        {
            BufferList = new ObservableCollection<ChatModel>();
            var BufferCount = count;
            int BufferIndex = 0;
            int maxBufferIndex = 20;
          
            if(BufferCount < maxBufferIndex)
            {
                maxBufferIndex = BufferCount;
            }
            if (BufferCount != 0)
            {
                for (int i = BufferCount; i >= 0; i--)
                {
                    
                    if (BufferIndex != maxBufferIndex)
                    {
                        BufferList.Add(MessageList[i]);
                        LastElement = i;
                        BufferIndex++;

                    }
                    else
                    {
                        break;
                    }

                }
                var count1 = BufferList.Count;
                for (int i = count1-1; i >= 0; i--)
                {
                    if(updateFlag)
                    {
                        LastMessage.Insert(0, BufferList[i]);
                    }
                    else
                    {
                        LastMessage.Add(BufferList[i]);
                       
                    }
                    
                   
                }
                BufferList = null;
                if (updateFlag != true)             
                    MessagingCenter.Send<ChatViewModel>(this, "Scrol");
            }
            


        }

        void ConnectToServerAsync()
        {
            ws = new WebSocketSharp.WebSocket(addressWS);

           ws.OnMessage += WsOnMEssage;
            ws.OnOpen += WsOnOpen;
            ws.Connect();
            DependencyService.Get<IAudioWebSocketCall>().InitAudioWebSocketCall(user.Id,user.receiverId);
          
           


        }

        class dataRoom {
            [JsonProperty("type")]
            public string subs { get; set; }
            [JsonProperty("sender_id")]
            public int sendr_id { get; set; }
            [JsonProperty("reciever_id")]
            public int reciever_id { get; set; }
        }

        private void WsOnOpen(object sender, EventArgs e)
        {
            var message = JsonConvert.SerializeObject(new dataRoom { subs = "subscribe", sendr_id = chat.Sender_id,reciever_id=user.receiverId });
            ws.Send(message);
        }
    }
}
