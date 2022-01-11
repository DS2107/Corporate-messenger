
using Corporate_messenger.Models;
using Corporate_messenger.Models.Chat;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using WebSocketSharp;
using System.IO;
using Corporate_messenger.Service;
using Corporate_messenger.Service.Notification;
using System.Threading;

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

       
      
        private static string addressWS = "ws://192.168.0.105:6001";     
        private string Next_page_url { get; set; }
        private int CurrentPage { get; set; }
        /// <summary>
        /// Модель данных чат
        /// </summary>
        public ChatModel chat = new ChatModel();

        //Сокет 
        public  WebSocketSharp.WebSocket ws = new WebSocketSharp.WebSocket(addressWS);

        // Модель юзера
        public SpecialDataModel user = new SpecialDataModel();

        private ObservableCollection<ChatModel> messageList = new ObservableCollection<ChatModel>();
       
        /// <summary>
        /// Список сообщений
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
        private bool firstupdate = false;
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

        INavigation navigate;
        /// <summary>
        /// Конструктор 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        public ChatViewModel(int id, string title, INavigation nav){
            navigate = nav;
            ws =  DependencyService.Get<ISocket>().MyWebSocket;
            ws.OnMessage += WsOnMEssage;
            chat.Chat_room_id = id;
            chat.Sender_id = user.Id;
            chat.SourceImage = "play.png";
            // создаем новый поток
            // Thread myThread = new Thread(new ParameterizedThreadStart(SendToken_GetChatsAsync));
            //myThread.Start("/api/chat/" + chat.Chat_room_id + "/" + user.Id + "/dialog"); // запускаем поток
            Next_page_url = "/api/chat/" + chat.Chat_room_id + "/" + user.Id + "/dialog";
            ThreadMessage = new Thread(new ThreadStart(ThreadFunc_GetMessage));
            ThreadMessage.Start();
           // Next_page_url = null;
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
        Thread ThreadMessage;
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
                    if (Next_page_url != null)
                    {
                        Next_page_url = Next_page_url.Substring(25);
                        ThreadMessage = new Thread(new ThreadStart(ThreadFunc_GetMessage));
                        ThreadMessage.Start(); 
                    }                
                       
                    IsRefreshing = false;
                });

            }
        }

        private async void ThreadFunc_GetMessage()
        {
            
               
               await  SendToken_GetChatsAsync(Next_page_url);
            
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
                Device.StartTimer(new TimeSpan(0, 0, 0, 0,1), () =>{
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

      
       private  async Task SendToken_GetChatsAsync(string url){

            // получаем данные в виде Ключ-Значение 
            JObject contentJobjects = await GetContent_Json(url);
            try
            {
                // По ключам получаем значения
                foreach (var KeyJobject in contentJobjects){
                    switch(KeyJobject.Key){
                        case "dialog":
                            Dialog(KeyJobject.Value);
                            break;
                        case "receiver_id":
                            user.receiverId = (int)KeyJobject.Value;
                            break;
                        case "pagination":
                            Pagination(KeyJobject.Value);
                            break;
                    }          
                }
                ThreadMessage.Abort();
            }
            catch(Exception ex)
            {

            }

        }
       
        private void Dialog(JToken value)
        {
            var ValueJobject = JsonConvert.SerializeObject(value);
            var Json_obj = JArray.Parse(ValueJobject);
            if (Next_page_url != null)
            {
                
                    AddFirstElemToListMessage(Json_obj);  
            }
            if (firstupdate == false)
            {
                MessageList = JsonConvert.DeserializeObject<ObservableCollection<ChatModel>>(ValueJobject);

                foreach (var item in MessageList)
                {
                    SetStartParametr_Message(item);
                }
                firstupdate = true;
            }
              


        }

        private void Pagination(JToken value)
        {
            var ValueJobject = JsonConvert.SerializeObject(value);
            dynamic Json_obj = JObject.Parse(ValueJobject);

            Next_page_url = (string)Json_obj.next_page_url;
            CurrentPage = (int)Json_obj.currentPage;
        }
        private void AddFirstElemToListMessage(JArray Json_obj)
        {
            foreach (var item_Jarray in Json_obj)
            {
                MessageList.Insert(0, SetStartParametr_Message(JsonConvert.DeserializeObject<ChatModel>(item_Jarray.ToString())));
            }
        }
        private ChatModel SetStartParametr_Message(ChatModel item)
        {
            if (item.Audio != null)
            {
                item.ValueSlider = 0.0;
                item.MaximumSlider = 1;
                item.IsMessageVisible = false;
                item.IsAuidoVisible = true;
                item.SourceImage = "play.png";
            }
            else
            {
                item.ValueSlider = 0.0;
                item.MaximumSlider = 1;
                item.IsMessageVisible = true;
                item.IsAuidoVisible = false;
            }
            return item;
        }
        private async Task<JObject> GetContent_Json(string url)
        {
            // Устанавливаем соеденение 
            HttpClient client = new HttpClient();

            // Тип Запроса
            var httpMethod = HttpMethod.Get;
            var address = DependencyService.Get<IFileService>().CreateFile() + url;

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

            return contentJobjects;
        }
         
        private void WsOnOpen(object sender, EventArgs e){         
            var message = JsonConvert.SerializeObject(new { 
                type = "subscribe", 
                sender_id = chat.Sender_id, 
                reciever_id = user.receiverId 
            });
            ws.Send(message);
           
        }
       
        private void WsOnMEssage(object sender, MessageEventArgs e)
        {
            dynamic Json_obj = JObject.Parse(e.Data);
            if ((string)Json_obj.type == "message")
            {
                ChatModel new_message = JsonConvert.DeserializeObject<ChatModel>(e.Data);

                if(new_message.Audio == null)
                 MessageList.Add(SetStartParametr_Message(new_message));
                else
                {
                    MessageList.Add(SetStartParametr_Message(new_message));
                    DependencyService.Get<IFileService>().SaveFile(new_message.Audio);
                }
                MessagingCenter.Send<ChatViewModel>(this, "Scrol");
            }

        }
       
    }
}
