using Corporate_messenger.Models;
using Corporate_messenger.Models.UserData;
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
    class CreateGroupPageViewModel: INotifyPropertyChanged
    {
        

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        // Специальные данные пользователя
        SpecialDataModel user = new SpecialDataModel();

        public ObservableCollection<FriendsModel> friends = new ObservableCollection<FriendsModel>();
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
  
        public CreateGroupPageViewModel() {
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
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri("http://192.168.0.105:8098/api/user/" +user.Id+"/friends"),
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
    }
}
