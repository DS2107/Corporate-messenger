
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

    class ChatViewModel : ApiAbstract, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        // флаг Включения аудио
        private bool PlayStopStart = true;
        // Элемент который сейчас воспроизводится 
        private ChatModel PLayItem;     
        // Данные пользователя
        private UserDataModel MyUser;
        // Сокет
        public WebSocketSharp.WebSocket ws = new WebSocketSharp.WebSocket(addressWS);

        // След страница чата
        private string Next_page_url { get; set; }
        // Текущая страница
        private int CurrentPage { get; set; }
        // Id Отправителя
        public int Rec_id{get;set;}

       
       
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
        private bool isRefreshing { get; set; }
        /// <summary>
        /// Конструктор с параметрами 
        /// </summary>
        /// <param name="chat_room_id"></param>
        /// <param name="title_room"></param>
        public ChatViewModel(int chat_room_id, string title_room,int user_id ){
            MyTimer();


            chat.Chat_room_id = chat_room_id;
            chat.Sender_id = user_id;
            chat.SourceImage = "play.png";


            ws = DependencyService.Get<ISocket>().MyWebSocket;
            ws.OnMessage += WsOnMEssage;

            Next_page_url = "/api/chat/" + chat_room_id + "/" + user_id + "/dialog";
            ThreadMessage = new Thread(new ThreadStart(ThreadFunc_GetMessage));
            ThreadMessage.Start();

        }

        private async void ThreadFunc_GetMessage()
        {
            await SendToken_GetChatsAsync(Next_page_url);

        }


        /// <summary>
        /// Отправить сообщение
        /// </summary>
        public  ICommand  SendMessage{
            get{
                return new Command(async (object obj) =>{
              
                    if (Input_message != null){
                        byte[] audio = null;
                        MyUser = await UserDbService.GetUser();
                       
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
                return new Command(() =>
                {
                    IsRefreshing = true;
                    if (Next_page_url != null)
                    {
                        var array_string = Next_page_url.Split("/a");
                        Next_page_url = "/a"+array_string[1];
                        ThreadMessage = new Thread(new ThreadStart(ThreadFunc_GetMessage));
                        ThreadMessage.Start();
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

                var message = JsonConvert.SerializeObject(new {
                    audio = audio,
                    sender_id = MyUser.Id,
                    receiver_id = Rec_id,
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

        
        private async Task SendToken_GetChatsAsync(string url)
        {
            try
            {
                // получаем данные в виде Ключ-Значение 
                JObject contentJobjects = await GetInfo_HttpMethod_Get_Async(url);

                // По ключам получаем значения
                foreach (var KeyJobject in contentJobjects)
                {
                    switch (KeyJobject.Key)
                    {
                        case "dialog":
                            Dialog(KeyJobject.Value);
                            break;
                        case "receiver_id":
                            Rec_id = (int)KeyJobject.Value;
                            break;
                        case "pagination":
                            Pagination(KeyJobject.Value);
                            break;
                    }
                }
              
            }
            catch (Exception ex)
            {
                var s = ex;
               // DependencyService.Get<IForegroundService>().MyToast(ex.Message);
            }
            finally
            {
                ThreadMessage.Abort();
            }

        }

        private void Dialog(JToken value)
        {
            var ValueJobject = JsonConvert.SerializeObject(value);
            var Json_obj = JArray.Parse(ValueJobject);
            if (CurrentPage != 0)            
               AddFirstElemToListMessage(Json_obj);           
            else
            {
                MessageList = JsonConvert.DeserializeObject<ObservableCollection<ChatModel>>(ValueJobject);

                foreach (var item in MessageList)
                {
                    SetStartParametr_Message(item);
                }
              
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

        private  void WsOnMEssage(object sender, MessageEventArgs e)
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

        private void MyTimer()
        {
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
               if (DependencyService.Get<IForegroundService>().Flag_On_Off_Socket == false)
                {
                    DependencyService.Get<IForegroundService>().Flag_On_Off_Socket = true;
                    ws = DependencyService.Get<ISocket>().MyWebSocket;
                    ws.OnMessage += WsOnMEssage;
                }



                return true; // return true to repeat counting, false to stop timer
            });
        }




    }
}
