using Corporate_messenger.Models;
using Corporate_messenger.Service;
using Corporate_messenger.Service.Notification;
using Corporate_messenger.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Corporate_messenger.ViewModels
{
    class ChatListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
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
        public ObservableCollection<ChatListModel> chatList = new ObservableCollection<ChatListModel>();
        /// <summary>
        /// Список друзей
        /// </summary>
        public ObservableCollection<ChatListModel> ChatList
        {
            get { return chatList; }
            set
            {
                if (chatList != value)
                {
                    chatList = value;
                    OnPropertyChanged("ChatList");
                }
            }
        }
        SpecialDataModel user = new SpecialDataModel();

        /// <summary>
        /// Конструктор
        /// </summary>
        public ChatListViewModel()
        {
            // chatList = App.Database.GetItems();
            ChatList.Clear();
            _ = SendToken_GetChatsAsync();
            
            ChatList.CollectionChanged += ChatList_CollectionChanged;
          //  DependencyService.Get<IForegroundService>().StartService();
            //CallClass call = new CallClass();
            //call.LessPort();
        }

       

        private void ChatList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add: // если добавление
                    ChatListModel newUser = args.NewItems[0] as ChatListModel;
                    
                    break;
                case NotifyCollectionChangedAction.Remove: // если удаление
                    ChatListModel oldUser = args.OldItems[0] as ChatListModel;
                  
                    break;
                case NotifyCollectionChangedAction.Replace: // если замена
                    ChatListModel replacedUser = args.OldItems[0] as ChatListModel;
                    ChatListModel replacingUser = args.NewItems[0] as ChatListModel;
                 
                    break;
            }
        }
        public ICommand UpdateList {

            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;

                    await SendToken_GetChatsAsync();

                    IsRefreshing = false;
                });

            }
        }
        public INavigation Navigation { get; set; }
        
     


        /// <summary>
        /// ОТправить токен и получить Чаты
        /// </summary>
        /// <returns></returns>
        async Task SendToken_GetChatsAsync()
        {
           
            // Устанавливаем соеденение 
            HttpClient client = new HttpClient();


            // Тип Запроса
            var httpMethod = HttpMethod.Get;
            var address = DependencyService.Get<IFileService>().CreateFile() + "/api/user/" + user.Id + "/chatroom";

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(address),
                Method = httpMethod,

            };
            // Отправка заголовка
            request.Headers.Add("Authorization", "Bearer " + user.Token);

            try
            {
                // Отправка данных 
                var httpResponse = await client.SendAsync(request);
                // Ответ от сервера 
                var contenJSON = await httpResponse.Content.ReadAsStringAsync();

                //****** РАСШИФРОВКА_ОТВЕТА ******
                JObject contentJobjects = JObject.Parse(contenJSON);

                foreach (var KeyJobject in contentJobjects)
                {
                    if (KeyJobject.Key == "chats")
                    {
                        var ValueJobject = JsonConvert.SerializeObject(KeyJobject.Value);
                        chatList = null;
                        ChatList = JsonConvert.DeserializeObject<ObservableCollection<ChatListModel>>(ValueJobject);

                    }
                }


                /*// Добавить в базу последние элементы
                foreach (var item in ChatList)
                {
                    App.Database.SaveItem(item);
                }*/
            }
            catch(Exception ex)
            {

                var s = ex;
            }

           
           
        }
    }
}
