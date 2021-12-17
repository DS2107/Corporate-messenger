
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
using Corporate_messenger.Service.Notification;

namespace Corporate_messenger.ViewModels
{
   
    class ChatViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public int LastElement { get; set; }
        static string addressWS = "ws://192.168.0.105:6001";
        private string input_message { get; set; }

        /// <summary>
        /// Модель данных чат
        /// </summary>
        public ChatModel chat = new ChatModel();

         //Сокет 
        public  WebSocketSharp.WebSocket ws = new WebSocketSharp.WebSocket(addressWS);

        // Модель юзера
        public SpecialDataModel user = new SpecialDataModel();

        private ObservableCollection<ChatModel> messageList = new ObservableCollection<ChatModel>();
        private ObservableCollection<ChatModel> BufferList;
        private ObservableCollection<ChatModel> lastMessage = new ObservableCollection<ChatModel>();
        /// <summary>
        /// Список друзей общий
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

        /// <summary>
        /// Последние 20 сообщений
        /// </summary>
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

        /// <summary>
        /// Конструктор 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        public ChatViewModel(int id, string title){
            ws =  DependencyService.Get<ISocket>().MyWebSocket;
            chat.Chat_room_id = id;
            chat.Sender_id = user.Id;
            chat.SourceImage = "play.png";         
           _ = SendToken_GetChatsAsync();
        }
        
        /// <summary>
        /// Переназначение кнопки обратно
        /// </summary>
        public ICommand GoBack {
            get
            {
                return new Command(async (object obj) => {                
                   
                    await Shell.Current.GoToAsync("//chats_list", true);
                   
                });
            }
        }     
        /// <summary>
        /// Отправить сообщение
        /// </summary>
        public  ICommand  SendMessage{
            get{
                return new Command(async (object obj) =>{
                    if (Input_message != null){
                        byte[] audio = null;
                       await Task.Run(()=> SendMyMessage(audio));
                    }
                });
            }
        }
        bool updateFlag = false;
        /// <summary>
        /// Обновление списка
        /// </summary>
        public ICommand UpdateList
        {

            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;
                    updateFlag = true;
                    await Task.Run(() => LastMessageAdd(LastElement));
                   
                    updateFlag = false;
                    IsRefreshing = false;
                });

            }
        }
        bool PlayStopStart = true;
        
        ChatModel PLayItem;

        /// <summary>
        /// Воспроизведение аудио сообщения
        /// </summary>
        public ICommand PlayAudioMessage
        {
            get
            {
                return new Command(async (object obj) => {
                    if (obj is ChatModel item)
                    {
                        if (PLayItem != item && PLayItem != null)
                        {
                            PLayItem.IsEnableSlider = false;
                            Stop(PLayItem);
                            await Task.Run(() => Play(item));
                        }
                        else
                        {
                            if (PlayStopStart)
                                await Task.Run(() => Play(item));
                            else
                                Stop(item);
                        }
                    }
                });
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
       
        /// <summary>
        /// Метод для воспроизведения Audio сообщения
        /// </summary>
        /// <param name="item">сообщение</param>
        private void Play(ChatModel item){          
            string file = DependencyService.Get<IFileService>().SaveFile(item.Audio);
            if (File.Exists(file)){
                DependencyService.Get<IAudio>().PlayAudioFile(file);
                item.MaximumSlider = DependencyService.Get<IAudio>().GetFullTimeAudio();
                item.IsEnableSlider = true;
                item.SourceImage = "stop.png";

               // int count = 0;
                Device.StartTimer(new TimeSpan(0, 0, 0,1), () =>{
                        if (item.MaximumSlider != item.ValueSlider){
                            item.ValueSlider = DependencyService.Get<IAudio>().GetPositionAudio();
                            var s = DependencyService.Get<IAudio>().GetPositionAudio();
                            return true; // runs again, or false to stop
                        }
                        else{
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
        /// <summary>
        /// Отсановка аудио
        /// </summary>
        /// <param name="item"></param>
        private void Stop(ChatModel item){
            DependencyService.Get<IAudio>().StopAudioFile();
            item.SourceImage = "play.png";
            PlayStopStart = true;
        }

        /// <summary>
        /// Подключение к сокетам
        /// </summary>
        void ConnectToServer(){
            //ws = new WebSocketSharp.WebSocket(addressWS);
            ws.OnMessage += WsOnMEssage;
            //ws.OnOpen += WsOnOpen;
            //ws.Connect();
           
        }
        async Task SendToken_GetChatsAsync(){
            
            // Устанавливаем соеденение 
            HttpClient client = new HttpClient();

            // Тип Запроса
            var httpMethod = HttpMethod.Get;
            var address = DependencyService.Get<IFileService>().CreateFile() + "/api/chat/" + chat.Chat_room_id + "/" + user.Id + "/dialog";

            var request = new HttpRequestMessage(){
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
           
            foreach (var KeyJobject in contentJobjects){
                if (KeyJobject.Key == "dialog"){
                    var ValueJobject = JsonConvert.SerializeObject(KeyJobject.Value);
                    MessageList = JsonConvert.DeserializeObject<ObservableCollection<ChatModel>>(ValueJobject);
                  
                        foreach (var item in MessageList){
                            if (item.Audio != null){
                                item.ValueSlider = 1;
                                item.MaximumSlider = 1;
                                item.IsMessageVisible = false;
                                item.IsAuidoVisible = true;
                                item.SourceImage = "play.png";
                            }
                            else{
                            item.ValueSlider = 1;
                            item.MaximumSlider = 1;
                            item.IsMessageVisible = true;
                                item.IsAuidoVisible = false;
                            }   
                    }     
                }
                if (KeyJobject.Key == "receiver_id"){
                    dynamic dynamic_receiverID = KeyJobject.Value;    
                    user.receiverId = (int)dynamic_receiverID.receiver_id;
                  
                }
            }
            if (MessageList.Count > 20)
                LastMessageAdd(MessageList.Count - 1);
            else
                LastMessage = MessageList;

            
            ConnectToServer();
                      
        } 
        void LastMessageAdd(int count){
            BufferList = new ObservableCollection<ChatModel>();
            var BufferCount = count;
            int BufferIndex = 0;
            int maxBufferIndex = 20;
          
            if(BufferCount < maxBufferIndex){
                maxBufferIndex = BufferCount;
            }
            if (BufferCount != 0){
                for (int i = BufferCount; i >= 0; i--)
                {                  
                    if (BufferIndex != maxBufferIndex){
                        BufferList.Add(MessageList[i]);
                        LastElement = i;
                        BufferIndex++;

                    }
                    else{
                        break;
                    }

                }
                var count1 = BufferList.Count;
                for (int i = count1-1; i >= 0; i--){
                    if(updateFlag){
                        LastMessage.Insert(0, BufferList[i]);
                    }
                    else{
                        LastMessage.Add(BufferList[i]);
                       
                    }   
                }
                BufferList = null;
                if (updateFlag != true)             
                    MessagingCenter.Send<ChatViewModel>(this, "Scrol");
            }
        }

   

      
        private void WsOnOpen(object sender, EventArgs e){         
            var message = JsonConvert.SerializeObject(new { 
                type = "subscribe", 
                sender_id = chat.Sender_id, 
                reciever_id = user.receiverId 
            });
            ws.Send(message);
           
        }
        bool callback = false;
        private void WsOnMEssage(object sender, MessageEventArgs e)
        {
            dynamic Json_obj = JObject.Parse(e.Data);
            if ((string)Json_obj.type == "message")
            {
                ChatModel new_message = JsonConvert.DeserializeObject<ChatModel>(e.Data);
                if (new_message.Audio == null)
                {
                    new_message.MaximumSlider = 0.01;
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
                        var s = ex;
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
                    DependencyService.Get<IFileService>().SaveFile(new_message.Audio);

                MessagingCenter.Send<ChatViewModel>(this, "Scrol");
            }
               
            


        }
    }
}
