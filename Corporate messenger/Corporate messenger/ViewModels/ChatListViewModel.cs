using Corporate_messenger.Models;
using Corporate_messenger.Models.Abstract;
using Corporate_messenger.Service.Notification;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Corporate_messenger.ViewModels
{
    class ChatListViewModel : ApiAbstract, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public INavigation Navigation { get; set; }
       public Thread ThreadChats;
      
        /// <summary>
        /// Флаг Обновление списка
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
        public ObservableCollection<ChatListModel> chatList = new ObservableCollection<ChatListModel>();
        /// <summary>
        /// Конструктор
        /// </summary>
        public ChatListViewModel()
        {

           

            if (DependencyService.Get<IForegroundService>().SocketFlag == false)
              DependencyService.Get<IForegroundService>().StartService();
        
        }


     
        public ICommand UpdateList {

            get
            {
                return new Command( () =>
                {
                    IsRefreshing = true;

                   
                    ThreadChats = new Thread(new ThreadStart(ThreadFunc_GetMessage));
                    ThreadChats.Start();
                    IsRefreshing = false;
                });

            }
        }
        public async void ThreadFunc_GetMessage()
        {
            await SendToken_GetChatsAsync();
        }        

        /// <summary>
        /// ОТправить токен и получить Чаты
        /// </summary>
        /// <returns></returns>
        private async Task SendToken_GetChatsAsync()
        {
                //****** РАСШИФРОВКА_ОТВЕТА ******
                JObject contentJobjects = await GetInfo_HttpMethod_Get_Async("/api/user/" + SpecDataUser.Id + "/chatroom");

                foreach (var KeyJobject in contentJobjects)
                {
                    switch (KeyJobject.Key)
                    {
                        case "chats":
                            var ValueJobject = JsonConvert.SerializeObject(KeyJobject.Value);
                            chatList = null;
                            ChatList = JsonConvert.DeserializeObject<ObservableCollection<ChatListModel>>(ValueJobject);
                            break;
                    }                 
                }
                ThreadChats.Abort();
         
         }     
    }
}
