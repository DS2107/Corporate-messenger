using Corporate_messenger.DB;
using Corporate_messenger.Models;
using Corporate_messenger.Models.Abstract;
using Corporate_messenger.Models.Chat;
using Corporate_messenger.Service;
using Corporate_messenger.Service.Notification;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WebSocketSharp;
using Xamarin.Forms;

namespace Corporate_messenger.ViewModels
{
    public class CacheChatViewModel :ApiAbstract, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        private ChatListModel SelectChat;
        // флаг Включения аудио
        private bool PlayStopStart = true;
        // Элемент который сейчас воспроизводится 
        private ChatModel PLayItem;
        // Данные пользователя
        private UserDataModel MyUser;
        // Сокет
        public WebSocketSharp.WebSocket ws = new WebSocketSharp.WebSocket(addressWS);
        public int Rec_id { get; set; } 
        public string TitlePage { get; set; }
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
        private bool isRefreshing { get; set; }
        ObservableCollection<ChatModel> messageList { get; set; }
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
        public CacheChatViewModel(ChatListModel item)
        {
            MyTimer();
            ws = DependencyService.Get<ISocket>().MyWebSocket;
            ws.OnMessage += WsOnMEssage;
            // выбранный чат 
            SelectChat = item;
            TitlePage = SelectChat.Title;
            Task.Run(()=>CheckDB()).Wait();
        }
        /// <summary>
        /// Отправить сообщение
        /// </summary>
        public ICommand SendMessage
        {
            get
            {
                return new Command(async (object obj) => {

                    if (Input_message != null)
                    {
                        byte[] audio = null;
                        MyUser = await UserDbService.GetUser();

                        SendMyMessage(audio);
                    }
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

                var message = JsonConvert.SerializeObject(new
                {
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
            catch (Exception ex)
            {
                var except = ex;
                DependencyService.Get<IFileService>().MyToast("Не удалось отправить сообщение");
            }


        }

        /// <summary>
        /// Метод для воспроизведения Audio сообщения
        /// </summary>
        /// <param name="item">сообщение</param>
        private void Play(ChatModel item)
        {
            string file = DependencyService.Get<IFileService>().SaveFile(item.Audio);
            if (File.Exists(file))
            {
                DependencyService.Get<IAudio>().PlayAudioFile(file);
                item.MaximumSlider = DependencyService.Get<IAudio>().GetFullTimeAudio();
                item.IsEnableSlider = true;
                item.SourceImage = "stop.png";

                // int count = 0;
                Device.StartTimer(new TimeSpan(0, 0, 0, 0, 1), () => {
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
        /// <summary>
        /// Отсановка аудио
        /// </summary>
        /// <param name="item"></param>
        private void Stop(ChatModel item)
        {
            DependencyService.Get<IAudio>().StopAudioFile();
            item.SourceImage = "play.png";
            PlayStopStart = true;
        }
        private void WsOnMEssage(object sender, MessageEventArgs e)
        {
            dynamic Json_obj = JObject.Parse(e.Data);
            if ((string)Json_obj.type == "message")
            {
                ChatModel new_message = JsonConvert.DeserializeObject<ChatModel>(e.Data);

                if (new_message.Audio == null)
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
                MessagingCenter.Send<CacheChatViewModel>(this, "Scrol");
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
                    var firstOrDefault = MessageList.FirstOrDefault();
                    var temp_list = await ChatDbService.GetOldMessage(firstOrDefault.message_id, SelectChat.Id);
                    if (temp_list.Count != 0)
                    {
                        foreach (var item in temp_list.Reverse())
                        {
                            SetStartParametr_Message(item);
                            MessageList.Insert(0, item);
                        }

                    }
                    else
                    {
                        var user = await UserDbService.GetUser();
                        var last_message = MessageList.FirstOrDefault();
                        string query = "/api/chat/" + SelectChat.Id + "/"+user.Id+"/"+last_message.message_id+"/old";
                        await SendToken_GetChatsAsync(query);
                    }
                   
                    IsRefreshing = false;
                });

            }
        }
        private bool FlagNewMessage { get; set; }
        private async Task CheckDB()
        {
            MessageList = await ChatDbService.GetLastMessages(SelectChat.Id);
            var User_Id = UserDbService.GetUser();
            if (MessageList.Count == 0)
            {
                FlagNewMessage = false;
                await SendToken_GetChatsAsync("/api/chat/" + SelectChat.Id + "/" + User_Id.Id + "/dialog");
            }
            else
            {
                FlagNewMessage = true;
                var lastElem = MessageList.LastOrDefault();
                var query = "/api/chat/" + SelectChat.Id + "/" + User_Id.Id + "/" + lastElem.message_id;
                await SendToken_GetChatsAsync(query);
            }
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
                           await Dialog(KeyJobject.Value);
                            break;
                        case "receiver_id":
                            Rec_id = (int)KeyJobject.Value;
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
              //  ThreadMessage.Abort();
            }

        }
        private async Task Dialog(JToken value)
        {
            var ValueJobject = JsonConvert.SerializeObject(value);
             if(MessageList.Count ==0)
            {
                MessageList = JsonConvert.DeserializeObject<ObservableCollection<ChatModel>>(ValueJobject);
                foreach (var item in MessageList)
                {

                    SetStartParametr_Message(item);

                    await ChatDbService.AddMessage(item);
                }
            }                
            else
            {
                if(FlagNewMessage == false)
                {
                    var TempList = JsonConvert.DeserializeObject<ObservableCollection<ChatModel>>(ValueJobject);
                    foreach (var item in TempList.Reverse())
                    {

                        SetStartParametr_Message(item);
                        MessageList.Insert(0, item);
                        await ChatDbService.AddMessage(item);
                    }
                }
                else
                {
                    var TempList = JsonConvert.DeserializeObject<ObservableCollection<ChatModel>>(ValueJobject);
                    foreach (var item in TempList)
                    {

                        SetStartParametr_Message(item);
                        MessageList.Add( item);
                        await ChatDbService.AddMessage(item);
                    }

                }
               
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
                item.Chat_room_id = SelectChat.Id;
            }
            else
            {
                item.ValueSlider = 0.0;
                item.MaximumSlider = 1;
                item.IsMessageVisible = true;
                item.IsAuidoVisible = false;
                item.Chat_room_id = SelectChat.Id;
            }
            return item;
        }

    }
}
