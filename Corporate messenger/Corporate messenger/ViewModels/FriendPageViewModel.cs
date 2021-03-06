using Corporate_messenger.DB;
using Corporate_messenger.Models;
using Corporate_messenger.Models.Abstract;
using Corporate_messenger.Models.UserData;
using Corporate_messenger.Service;
using Corporate_messenger.Service.Notification;
using Corporate_messenger.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Corporate_messenger.ViewModels
{

    class FriendPageViewModel :ApiAbstract, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        string ValueJobject = "";

        //Поток
        Thread myThread = null;


        /// <summary>
        /// Список друзей
        /// </summary>
        public ObservableCollection<FriendsModel> Friends
        {
            get { return friends; }
            set
            {
                if (friends != value)
                {
                    friends = value;
                    OnPropertyChanged("Friends");
                }
            }
        }
        private ObservableCollection<FriendsModel> friends = new ObservableCollection<FriendsModel>();

        /// <summary>
        /// Навигация
        /// </summary>
        private INavigation nav { get; set; }

        public ICommand NewChatButton
        {
            get{
                return new Command(async (object obj) =>{
                    // Ищем нужный элемент
                    if (obj is FriendsModel item){
                       
                        // Перед отправкой , превращаем все в json "/api/chatroom"
                        string jsonLog = JsonConvert.SerializeObject(new { sender_id = User.Id, receiver_id = item.Id, title = item.Name });

                        //****** РАСШИФРОВКА_ОТВЕТА ******//
                        dynamic contentJobjects = await GetInfo_HttpMethod_Post_Async(jsonLog, "/api/chatroom",false);

                        if (contentJobjects == null)
                            DependencyService.Get<IFileService>().MyToast("Отсутствует соеденение с сервером, проверьте подключение к интернету и потворите попытку");
                        else
                            // Переход на следующую страницу
                            await nav.PushAsync(new ChatPage((int)contentJobjects.chat_room_id, item.Name));
            
                    }
                });
            }
        }
        public ICommand Call
        {
            get
            {
                return new Command(async (object obj) => {
                    // Ищем нужный элемент
                    if (obj is FriendsModel item)
                    {

                       


                        CallPage callPage = new CallPage(true);
                        callPage.SetName(item.Name);
                        await nav.PushAsync(callPage);
                        try
                        {
                            DependencyService.Get<IAudioUDPSocketCall>().ConnectionToServer();
                            var s = DependencyService.Get<IAudioUDPSocketCall>().GetServerIp();
                            DependencyService.Get<ISocket>().MyWebSocket.Send(JsonConvert.SerializeObject(new
                            {
                                type = "init_call",
                                status = "100",
                                sender_id = User.Id,
                                receiver_id = item.Id,
                                call_address = DependencyService.Get<IAudioUDPSocketCall>().GetServerIp()
                            }));
                            DependencyService.Get<IAudio>().PlayAudioFile("gudok.mp3", Android.Media.Stream.VoiceCall);
                            DependencyService.Get<IForegroundService>().Flag_AudioCalls_Init = true;


                        }
                        catch (Exception ex)
                        {
                            DependencyService.Get<IForegroundService>().MyToast("Не удается позвонить, возможно потеряно соединение с сервором: " + ex.Message);
                        }

                    }
                });
            }
        }



        UserDataModel User;
        public FriendPageViewModel(INavigation page) {
            nav = page;
            Task.Run(() => GetUser()).Wait();
          Task.Run(()=>  SendToken_GetFriendsAsync()).Wait(); 
        }
      private async Task GetUser()
        {
            User = await UserDbService.GetUser();
        }
        /// <summary>
        /// ОТправить токен и получить друзей
        /// </summary>
        /// <returns></returns>
        async Task SendToken_GetFriendsAsync()
        {
            //****** РАСШИФРОВКА_ОТВЕТА ******
            JObject contentJobjects = await GetInfo_HttpMethod_Get_Async("/api/user/" + User.Id + "/friends");
            foreach (var KeyJobject in contentJobjects)
            {
                switch (KeyJobject.Key){
                    case "friends":
                         ValueJobject = JsonConvert.SerializeObject(KeyJobject.Value);
                        myThread = new Thread(FillList);
                        myThread.Start();
                        //Friends = JsonConvert.DeserializeObject<ObservableCollection<FriendsModel>>(ValueJobject);
                        break;
                }              
            }
        }
        public  void FillList()
        {
            Friends.Clear();
            Friends = JsonConvert.DeserializeObject<ObservableCollection<FriendsModel>>(ValueJobject);
            myThread.Abort();
        }
     
        public async Task SearchFriendAsync(string userParam)
        {
            //****** РАСШИФРОВКА_ОТВЕТА ******
            JObject contentJobjects = await GetInfo_HttpMethod_Get_Async("/api/user/" + User.Id + "/search/" + userParam);

            foreach (var KeyJobject in contentJobjects)
            {
                switch (KeyJobject.Key) {
                    case "user":
                            ValueJobject = JsonConvert.SerializeObject(KeyJobject.Value);
                            myThread = new Thread(FillList);
                            myThread.Start();
                        break;
                    case "error":
                            Friends.Clear();
                        break;
                } 
            }
        }


    }

   
}
