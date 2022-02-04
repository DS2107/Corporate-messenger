
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
using Corporate_messenger.Models.Abstract;
using Corporate_messenger.DB;
using System.Linq;
using Sockets.Plugin;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace Corporate_messenger.ViewModels
{
   
    class ChatViewModel: ApiAbstract, INotifyPropertyChanged
    {
     
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        bool PlayStopStart = true;
        ChatModel PLayItem;
        INavigation navigate;
       
        private bool firstupdate = false;
        public string Next_page_url { get; set; }
        private int CurrentPage { get; set; }

       
       
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
        private ObservableCollection<ChatModel> messageList = new ObservableCollection<ChatModel>();

       
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
        private string input_message { get; set; }


        /// <summary>
        /// Флаг обновления списка
        /// </summary>
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
        private bool isRefreshing = false;
        UserDataModel user;
        public WebSocketSharp.WebSocket ws = new WebSocketSharp.WebSocket(addressWS);
        UdpSocketClient client;
        /// <summary>
        /// Конструктор с параметрами 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        public ChatViewModel(int id, string title, INavigation nav){
            
            navigate = nav;
           
            chat.Chat_room_id = id;
            chat.Sender_id = SpecDataUser.Id;
            chat.SourceImage = "play.png";
            SpecDataUser.Input_chat = chat.Chat_room_id;

           
        }

      


        /// <summary>
        /// Отправить сообщение
        /// </summary>
        public  ICommand  SendMessage{
            get{
                return new Command(async (object obj) =>{
              
                    if (Input_message != null){
                        byte[] audio = null;
                        user = await UserDbService.GetUser();
                       
                        SendMyMessage(audio);
                    }
                });
            }
        }
        public Thread ThreadMessage;
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
                   
                    var user = await UserDbService.GetUser();
                    var sss = MessageList.First().message_id;
                    contentJobjects = await GetInfo_HttpMethod_Get_Async("/api/chat/" + chat.Chat_room_id + "/" + user.Id + "/" + MessageList.First().message_id +"/old");
                        if (contentJobjects != null)
                        {
                            ThreadMessage = new Thread(new ThreadStart(RequestOldMessage));
                            ThreadMessage.Start();
                        }
                        else
                        {
                          
                            DependencyService.Get<IFileService>().MyToast("Отсутствует соеденение с сервером, проверьте подключение к интернету и потворите попытку");
                        }
                       
                                   
                       
                    IsRefreshing = false;
                });

            }
        }

       

        

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
            try
            {
               
               
                ws = DependencyService.Get<ISocket>().MyWebSocket;
                ws.OnMessage += WsOnMEssage;
                var message = JsonConvert.SerializeObject(new {
                    audio = audio,
                    sender_id = user.Id,
                    receiver_id = SpecDataUser.receiverId,
                    type = "message",
                    message = Input_message,
                    chat_room_id = chat.Chat_room_id,  
                 });
                ws.Send(message);
               
                Input_message = "";
            }
            catch(Exception ex)
            {
                var except = ex;
                DependencyService.Get<IFileService>().MyToast("Не удалось отправить сообщение");
            }
           
            
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

        public JObject contentJobjects;
       public  void SendToken_GetChats(){
            try
            {
               
                if(contentJobjects == null)
                {
                    DependencyService.Get<IFileService>().MyToast("Отсутствует соеденение с сервером, проверьте подключение к интернету и потворите попытку");
                }
                else
                {
                    // По ключам получаем значения
                    foreach (var KeyJobject in contentJobjects)
                    {
                        switch (KeyJobject.Key)
                        {
                            case "dialog":
                                Dialog(KeyJobject.Value);
                                break;
                            case "receiver_id":
                                SpecDataUser.receiverId = (int)KeyJobject.Value;
                                break;
                            case "pagination":
                                Pagination(KeyJobject.Value);
                                break;
                        }
                    }
                }
             
            }
            catch(Exception ex)
            {
                DependencyService.Get<IForegroundService>().MyToast(ex.Message);
            }
            finally{
                ThreadMessage.Abort();
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
                item.Chat_room_id = chat.Chat_room_id;
            }
            else
            {
                item.ValueSlider = 0.0;
                item.MaximumSlider = 1;
                item.IsMessageVisible = true;
                item.IsAuidoVisible = false;
                item.Chat_room_id = chat.Chat_room_id;
            }
            return item;
        }
       
         
      
       
        private async void WsOnMEssage(object sender, MessageEventArgs e)
        {
            dynamic Json_obj = JObject.Parse(e.Data);
            if ((string)Json_obj.type == "message")
            {
                ChatModel new_message = JsonConvert.DeserializeObject<ChatModel>(e.Data);

                if(new_message.Audio == null)
                {
                    MessageList.Add(SetStartParametr_Message(new_message));
                   // await ChatDbService.AddMessage(new_message);
                }                
                else
                {
                    MessageList.Add(SetStartParametr_Message(new_message));
                   // await ChatDbService.AddMessage(new_message);
                    DependencyService.Get<IFileService>().SaveFile(new_message.Audio);
                }
                MessagingCenter.Send<ChatViewModel>(this, "Scrol");
            }

        }

        ObservableCollection<ChatModel> sql_message = new ObservableCollection<ChatModel>();
       public async Task GetSql(int chat_room)
        {
             sql_message = await ChatDbService.GetMessages(chat_room);
            var user = await UserDbService.GetUser();
            if(sql_message.Count == 0)
            {
                contentJobjects = await GetInfo_HttpMethod_Get_Async("/api/chat/" + chat_room + "/" + user.Id + "/dialog");
                if (contentJobjects != null)
                {
                    ThreadMessage = new Thread(new ThreadStart(RequestFirstMessage));
                    ThreadMessage.Start();
                }
                else
                {
                    DependencyService.Get<IFileService>().MyToast("Отсутствует соеденение с сервером, проверьте подключение к интернету и потворите попытку");
                }
            }
            else
            {
                MessageList = sql_message;
                var ss = MessageList.Last().message_id;
                contentJobjects = await GetInfo_HttpMethod_Get_Async("/api/chat/" + chat_room + "/" + user.Id + "/"+MessageList.Last().message_id);
                if (contentJobjects != null)
                {
                    ThreadMessage = new Thread(new ThreadStart(RequestNewMessage));
                    ThreadMessage.Start();
                }
                else
                {
                    DependencyService.Get<IFileService>().MyToast("Отсутствует соеденение с сервером, проверьте подключение к интернету и потворите попытку");
                }
            }

        }
        public async void RequestNewMessage()
        {        
                if(contentJobjects != null)
                {
                    // По ключам получаем значения
                    foreach (var KeyJobject in contentJobjects)
                    {
                        switch (KeyJobject.Key)
                        {
                            case "dialog":
                                var ValueJobject = JsonConvert.SerializeObject(KeyJobject.Value);
                                sql_message = JsonConvert.DeserializeObject<ObservableCollection<ChatModel>>(ValueJobject);
                                foreach (var item in sql_message)
                                {
                                    await ChatDbService.AddMessage(SetStartParametr_Message(item));
                                    MessageList.Add(SetStartParametr_Message(item));
                                }
                           
                                break;
                        }
                    }
                } 
        }
        public async void RequestOldMessage()
        {
            if (contentJobjects != null)
            {
                // По ключам получаем значения
                foreach (var KeyJobject in contentJobjects)
                {
                    switch (KeyJobject.Key)
                    {
                        case "dialog":
                            var ValueJobject = JsonConvert.SerializeObject(KeyJobject.Value);
                            sql_message = JsonConvert.DeserializeObject<ObservableCollection<ChatModel>>(ValueJobject);
                            if (sql_message.Count != 0)
                            {
                                await ChatDbService.DeleteAllMessage();
                                foreach (var item in sql_message)
                                {
                                    //await ChatDbService.AddMessage(SetStartParametr_Message(item));
                                    MessageList.Add(SetStartParametr_Message(item));
                                }
                                foreach(var item in MessageList)
                                {
                                    await ChatDbService.AddMessage(item);
                                }
                            }
                           

                            break;
                    }
                }
            }
        }
        public async void RequestFirstMessage()
        {
            try
            {
                if (contentJobjects == null)
                {
                    DependencyService.Get<IFileService>().MyToast("Отсутствует соеденение с сервером, проверьте подключение к интернету и потворите попытку");
                }
                else
                {
                    // По ключам получаем значения
                    foreach (var KeyJobject in contentJobjects)
                    {
                        switch (KeyJobject.Key)
                        {
                            case "dialog":
                                var ValueJobject = JsonConvert.SerializeObject(KeyJobject.Value);
                                sql_message = JsonConvert.DeserializeObject<ObservableCollection<ChatModel>>(ValueJobject);
                                foreach(var item in sql_message)
                                {

                                    await ChatDbService.AddMessage(SetStartParametr_Message(item));
                                }
                                MessageList = await ChatDbService.GetMessages(chat.Chat_room_id);
                                break;
                            case "receiver_id":
                                SpecDataUser.receiverId = (int)KeyJobject.Value;
                                break;
                            case "pagination":
                                Pagination(KeyJobject.Value);
                                break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var exc = ex;
              //  DependencyService.Get<IForegroundService>().MyToast(ex.Message);
            }
            finally
            {
                ThreadMessage.Abort();
            }

        }

    }
}
