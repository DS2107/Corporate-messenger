using Corporate_messenger.Models;
using Corporate_messenger.Models.UserData;
using Corporate_messenger.Service;
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
     class ModelCreate
    {
        public int sender_id { get; set; }

        public int receiver_id { get; set; }

        public string title { get; set; }
    }

    class MyModel
    {
       

        public int chat_room_id { get; set; }

        public string status { get; set; }
    }

    class FriendPageViewModel : INotifyPropertyChanged
    {
        

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        // Специальные данные пользователя
        SpecialDataModel user = new SpecialDataModel();

        private ObservableCollection<FriendsModel> friends = new ObservableCollection<FriendsModel>();
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

      

        SpecialDataModel iUser =  new  SpecialDataModel();
            public ICommand NewChatButton
        {

            get
            {
                return new Command(async (object obj) =>
                {
                    if (obj is FriendsModel item)
                    {
                        // Модель авторизации
                        ModelCreate log = new ModelCreate();
                        log.sender_id = iUser.Id;
                        log.receiver_id = item.Id;
                        log.title = item.Username;

                      
                        // Перед отправкой , превращаем все в json
                        string jsonLog = JsonConvert.SerializeObject(log);

                        // Устанавливаем соеденение 
                        HttpClient client = new HttpClient();

                        // Тип данных который мы принимаем от сервера 
                        var contentType = "application/json";

                        // Тип Запроса
                        var httpMethod = HttpMethod.Post;
                        var address  = DependencyService.Get<IFileService>().CreateFile() + "/api/chatroom";
                        var request = new HttpRequestMessage()
                        {
                            RequestUri = new Uri(address),
                            Method = httpMethod,
                            Content = new StringContent(jsonLog, System.Text.Encoding.UTF8, contentType)
                        };
                        // Отправка заголовка
                        request.Headers.Add("Accept", "application/json");
                        request.Headers.Add("Authorization", "Bearer " + iUser.Token);
                        // Отправка данных авторизации
                        var httpResponse = await client.SendAsync(request);

                        // Ответ от сервера 
                        var contenJSON = await httpResponse.Content.ReadAsStringAsync();

                        //****** РАСШИФРОВКА_ОТВЕТА ******
                        var contentJobjects = JObject.Parse(contenJSON);
                        MyModel my = JsonConvert.DeserializeObject<MyModel>(contenJSON);
                       
                        nav.PushAsync(new ChatPage(my.chat_room_id,item.Username));
/*                            iUser.Id;
                        item.Username;
                        item.Id;*/
                    }


                });

            }
        }

        private INavigation nav { get; set; }
        public INavigation Nav
        {
            get { return nav; }
            set
            {
                if (nav != value)
                {
                    nav = value;
                    OnPropertyChanged("Nav");
                }
            }
        }
        public FriendPageViewModel(INavigation page) {
            Nav = page;
           _ =  SendToken_GetFriendsAsync(); 
        }
        
        /// <summary>
        /// ОТправить токен и получить друзей
        /// </summary>
        /// <returns></returns>
        async Task SendToken_GetFriendsAsync()
        {
            // Устанавливаем соеденение 
            HttpClient client = new HttpClient();

            // Тип Запроса
            var httpMethod = HttpMethod.Get;
            var address = DependencyService.Get<IFileService>().CreateFile() + "/api/user/" +user.Id+ "/friends";

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(address),
                Method = httpMethod,
             
            };
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
                if (KeyJobject.Key == "friends")
                {
                    var ValueJobject = JsonConvert.SerializeObject(KeyJobject.Value);
                    Friends = JsonConvert.DeserializeObject<ObservableCollection<FriendsModel>>(ValueJobject);

                }
            }
        }

        public async Task SearchFriendAsync(string userParam)
        {
            // Устанавливаем соеденение 
            HttpClient client = new HttpClient();

            // Тип Запроса
            var httpMethod = HttpMethod.Get;
            var address = DependencyService.Get<IFileService>().CreateFile() + "/api/user/"+ user.Id+"/search/" + userParam;

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(address),
                Method = httpMethod,

            };
            // Отправка заголовка
            request.Headers.Add("Authorization", "Bearer " + user.Token);
            request.Headers.Add("Accept", "application/json");
            // Отправка данных 
            var httpResponse = await client.SendAsync(request);

            // Ответ от сервера 
            var contenJSON = await httpResponse.Content.ReadAsStringAsync();

            //****** РАСШИФРОВКА_ОТВЕТА ******
            JObject contentJobjects = JObject.Parse(contenJSON);

            foreach (var KeyJobject in contentJobjects)
            {
                if (KeyJobject.Key == "user")
                {
                    var ValueJobject = JsonConvert.SerializeObject(KeyJobject.Value);
                    Friends.Clear();
                    Friends = JsonConvert.DeserializeObject<ObservableCollection<FriendsModel>>(ValueJobject);

                }
                if (KeyJobject.Key == "error")
                {
                    Friends.Clear();
                }
            }
        }


    }

   
}
