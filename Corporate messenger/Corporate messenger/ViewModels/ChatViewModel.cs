
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

namespace Corporate_messenger.ViewModels
{
    class ChatViewModel: INotifyPropertyChanged
    {
        /// <summary>
        /// Клиент для связи с сокетом
        /// </summary>
        ClientWebSocket client = new ClientWebSocket();

        /// <summary>
        /// Модель данных чат
        /// </summary>
        ChatModel chat = new ChatModel();

         //Сокет 
         WebSocketSharp.WebSocket ws = new WebSocketSharp.WebSocket("ws://192.168.0.105:6001");

        // Модель юзера
        SpecialDataModel user = new SpecialDataModel();
      

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
            // Регестрирую команду для кнопки 
            SendMessage = new Command(SendMessageCommand);
            GoBack = new Command(GoBackCommand);
         
           
            MessageList.CollectionChanged += MessageList_CollectionChanged;
             ConnectToServerAsync();
           _ = SendToken_GetChatsAsync();
          
        }

       

        private void WsOnMEssage(object sender, MessageEventArgs e)
        {
          
                ChatModel new_message = JsonConvert.DeserializeObject<ChatModel>(e.Data);
            LastMessage.Add(new_message);
            MessagingCenter.Send<ChatViewModel>(this, "Scrol");
        }


      
        public void StopSendMessageAudioCommandAsync()
        {
           
            if (ClassMedia != null)
            {
                ClassMedia.Stop();
                ClassMedia.Release();
                ClassMedia = null;
            }
        }
        MediaRecorder ClassMedia;
        string folderPath = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "MyRecord");
       
       

        public async Task SendMessageAudioCommand()
        {
            
            // try
            //{
            // Даю разрешения для микрофона и (зписи/чтения файлов)
            var PermissionsStrorage = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            var PermissionsMicrophone = await Permissions.CheckStatusAsync<Permissions.Microphone>();

            // Прверка разрешенний
            if (PermissionsStrorage != PermissionStatus.Granted && PermissionsMicrophone != PermissionStatus.Granted)
            {
                PermissionsStrorage = await Permissions.RequestAsync<Permissions.StorageWrite>();
                PermissionsMicrophone = await Permissions.RequestAsync<Permissions.Microphone>();
            }
            if (PermissionsStrorage != PermissionStatus.Granted && PermissionsMicrophone != PermissionStatus.Granted)
            {
                return;
            }



         /*   if (!File.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (ClassMedia == null)
                ClassMedia = new MediaRecorder();
            else
                ClassMedia.Reset();

            ClassMedia.SetAudioSource(AudioSource.VoiceCall);
            ClassMedia.SetOutputFormat(OutputFormat.AmrNb);
            ClassMedia.SetAudioEncoder(AudioEncoder.AmrNb);
            ClassMedia.SetOutputFile(folderPath + "/" + "temp" + DateTime.Now.ToString("HH:mm:ss") + ".amr");



            ClassMedia.Prepare();
            ClassMedia.Start();*/
        }

        /// <summary>
        /// Команда для кнопки отправки сообщения
        /// </summary>
        public ICommand SendMessage { get; set; }
        public void SendMessageCommand(object obj)
        {
            SendAsync();
        }

        /// <summary>
        /// Отправка сообщения
        /// </summary>
        /// <returns></returns>
        public void SendAsync()
        {
            var new_message = new ChatModel { Chat_room_id = chat.Chat_room_id, Sender_id = chat.Sender_id, Message = Input_message, Receiver_id = user.receiverId, TypeMessage = "message" };
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

        async Task SendToken_GetChatsAsync()
        {
            
            // Устанавливаем соеденение 
            HttpClient client = new HttpClient();


            // Тип Запроса
            var httpMethod = HttpMethod.Get;
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri("http://192.168.0.105:8098/api/chat/" + chat.Chat_room_id +"/"+user.Id+ "/dialog"),
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
            LastMessageAdd(MessageList.Count-1);
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
            ws = new WebSocketSharp.WebSocket("ws://192.168.0.105:6001/app");

            ws.OnMessage += WsOnMEssage;
            ws.OnOpen += WsOnOpen;
            ws.Connect();
           


        }

        class dataRoom {
            [JsonProperty("type")]
            public string subs { get; set; }
            [JsonProperty("sender_id")]
            public int sendr_id { get; set; }
        }

        private void WsOnOpen(object sender, EventArgs e)
        {
            var message = JsonConvert.SerializeObject(new dataRoom { subs = "subscribe", sendr_id = chat.Sender_id });
            ws.Send(message);
        }
    }
}
