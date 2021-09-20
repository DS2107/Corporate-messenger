using Corporate_messenger.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
        public ChatListViewModel()
        {
           _ = SendToken_GetChatsAsync();
        }
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
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri("http://192.168.0.105:8098/api/user/" + user.Id + "/chatroom"),
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
                if (KeyJobject.Key == "chats")
                {
                    var ValueJobject = JsonConvert.SerializeObject(KeyJobject.Value);
                    ChatList = JsonConvert.DeserializeObject<ObservableCollection<ChatListModel>>(ValueJobject);

                }
            }
        }
    }
}
