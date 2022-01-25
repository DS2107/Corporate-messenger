using Corporate_messenger.DB;
using Corporate_messenger.Models;
using Corporate_messenger.Models.Abstract;
using Corporate_messenger.Service;
using Corporate_messenger.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Corporate_messenger.ViewModels
{
    class ChatListViewModel : ApiAbstract, INotifyPropertyChanged
    {
       public UserDataModel user;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public INavigation Navigation { get; set; }

        public Thread ThreadChats;

        INavigation navigation;


     
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
            get { return chats; }
            set
            {
                if (chats != value)
                {
                    chats = value;
                    OnPropertyChanged("ChatList");
                }
            }
        }
       
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
                    user = await UserDbService.GetUser();
                    //****** РАСШИФРОВКА_ОТВЕТА ******
                    contentJobjects = await GetInfo_HttpMethod_Get_Async("/api/user/" + user.Id + "/chatroom");

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
                            

                        var buffer = JsonConvert.DeserializeObject<ObservableCollection<ChatListModel>>(ValueJobject);
                        if (ChatList.Count == 0)
                        {
                            ChatList = buffer;
                            foreach (var item in ChatList)
                            {
                                _ = ChatListDbService.AddChat(item);
                               // ChatList.Add(item);

                            }
                        }
                        else
                        {
                         
                            foreach (var item in buffer)
                            {
                                foreach(var EditItem in ChatList)
                                {
                                    if(EditItem.Id == item.Id && EditItem.Updated_at != item.Updated_at)
                                    {
                                        isRefreshing = true;
                                        EditItem.Last_message = item.Last_message;
                                        EditItem.Updated_at = item.Updated_at;
                                        _ = ChatListDbService.UpdateChat(EditItem);
                                    }
                                        
                                }
                               
                            }
                        }
                            IsRefreshing = false;
                           
                        break;
                    }
                }
                ThreadChats.Abort();
           
         }
       
        ObservableCollection<ChatListModel> chats ;
        public async Task GetSqlChats()
        {
            ChatList = null;
            ChatList = await ChatListDbService.GetChats();
            UserDataModel user = await UserDbService.GetUser();
            if (ChatList.Count != 0)
            {
                

                //****** РАСШИФРОВКА_ОТВЕТА ******
                contentJobjects = await GetInfo_HttpMethod_Get_Async("/api/user/" + user.Id + "/chatroom");
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
            }
            else
            {
                IsRefreshing = true;

                //****** РАСШИФРОВКА_ОТВЕТА ******
                contentJobjects = await GetInfo_HttpMethod_Get_Async("/api/user/" + user.Id + "/chatroom");
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
            }
        }


       
    }
}
