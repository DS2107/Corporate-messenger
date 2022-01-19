using Corporate_messenger.Models;
using Corporate_messenger.Models.Abstract;
using Corporate_messenger.Service;
using Corporate_messenger.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
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
        INavigation navigation;


        public string Name
        {
            get {

                
                return SpecDataUser.Name; }
            set
            {
                
                    SpecDataUser.Name = value;
                    OnPropertyChanged("Name");
                
            }

        }
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
        public ChatListViewModel(INavigation nav)
        {
            navigation = nav;
 
        }

        public ICommand GoFriends { 
            get
            {
                return new Command(async () =>
                await navigation.PushAsync(new FriendPage())
                );
            }
                
          }


        public ICommand UpdateList {

            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;

                    //****** РАСШИФРОВКА_ОТВЕТА ******
                    contentJobjects = await GetInfo_HttpMethod_Get_Async("/api/user/" + SpecDataUser.Id + "/chatroom");

                    if (contentJobjects == null)
                    {

                        DependencyService.Get<IFileService>().MyToast("Отсутствует соеденение с сервером, проверьте подключение к интернету и потворите попытку");
                        IsRefreshing = false;
                    }
                    else
                    {
                        ThreadChats = new Thread(new ThreadStart(SendToken_GetChats));
                        ThreadChats.Start();
                    }
                  
                    IsRefreshing = false;
                });

            }
        }
     
        public JObject contentJobjects;
        /// <summary>
        /// ОТправить токен и получить Чаты
        /// </summary>
        /// <returns></returns>
        public void  SendToken_GetChats()
        {

                foreach (var KeyJobject in contentJobjects)
                {
                    switch (KeyJobject.Key)
                    {
                        case "chats":
                            var ValueJobject = JsonConvert.SerializeObject(KeyJobject.Value);
                            chatList = null;
                            ChatList = JsonConvert.DeserializeObject<ObservableCollection<ChatListModel>>(ValueJobject);
                            IsRefreshing = false;
                        break;
                    }
                }
                ThreadChats.Abort();
           
         }     
    }
}
