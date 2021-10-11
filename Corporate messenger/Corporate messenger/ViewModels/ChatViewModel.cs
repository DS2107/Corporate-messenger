
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using WebSocketSharp;
using SocketIOClient;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Corporate_messenger.Views;

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
            MessageList.CollectionChanged += MessageList_CollectionChanged;
            ConnectToServerAsync();
            SendToken_GetChatsAsync();    
        }

     
       
        private void WsOnMEssage(object sender, MessageEventArgs e)
        {
            ChatModel new_message = JsonConvert.DeserializeObject<ChatModel>( e.Data);
            MessageList.Add(new_message);
            MessagingCenter.Send<ChatViewModel>(this, "Scrol");
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
        public async Task SendAsync()
        {
            var message = JsonConvert.SerializeObject(new ChatModel { Chat_room_id = chat.Chat_room_id, Sender_id = chat.Sender_id, Message = Input_message });
            ws.Send(message);
            Input_message = "";
            
        }


        async Task SendToken_GetChatsAsync()
        {
            
            // Устанавливаем соеденение 
            HttpClient client = new HttpClient();


            // Тип Запроса
            var httpMethod = HttpMethod.Get;
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri("http://192.168.0.105:8098/api/chat/" + chat.Chat_room_id + "/dialog"),
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
                    MessagingCenter.Send<ChatViewModel>(this, "Scrol");
                }
            }


           /* // Добавить в базу последние элементы
            foreach (var item in MessageList)
            {
                App.Database.SaveItem(item);
            }*/

        }

        async Task ConnectToServerAsync()
        {
            ws = new WebSocketSharp.WebSocket("ws://192.168.0.105:6001/app");

            ws.OnMessage += WsOnMEssage;
            ws.Connect();
           


        }

    }
}
