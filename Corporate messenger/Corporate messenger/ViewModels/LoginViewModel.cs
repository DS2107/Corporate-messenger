using Corporate_messenger.Models;
using Corporate_messenger.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Corporate_messenger.ViewModels
{
    public class LoginViewModel
    {

        public ICommand AuthorizationUserCommand { get; set; }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public LoginViewModel()
        {
            AuthorizationUserCommand = new Command(AuthorizationUserAsync);
           
        }

        /// <summary>
        /// Переход в нутрь приложения 
        /// </summary>
        /// <returns></returns>
        async Task GoChatListPageAsync()
        {
            await Shell.Current.GoToAsync("//chats_list", true);
        }

        private void AuthorizationUserAsync(object obj)
        {
            _ = AuthorizationUserAsync();
        }

        private async Task AuthorizationUserAsync( )
        {
            LogUser log = new LogUser();
            
           // var lo = log.Login;
           // var pa = log.Pass;

            // токен юзера который мы получим при успешной авторизации
            string UserToken = "";

            //ЛОГИРОВАНИЕ**********
            // Перед отправкой , превращаем все в json
            string jsonLog = JsonConvert.SerializeObject(log);
            HttpClient client = new HttpClient();
            var contentType = "application/json"; //May vary based on your app
            var httpMethod = HttpMethod.Post; //or Get, or whatever HTTP verb your API endpoint needs
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri("http://192.168.0.105:8098/api/login"),
                Method = httpMethod,
                Content = new StringContent(jsonLog, System.Text.Encoding.UTF8, contentType)
            };
            var httpResponse = await client.SendAsync(request);
            var contenJSON = await httpResponse.Content.ReadAsStringAsync();

            //РАСШИФРОВКА ОТВЕТА******
            JObject a = JObject.Parse(contenJSON);
            UserData userdata = null;
            string status = "";
            foreach (var o in a)
            {
                if (o.Key == "status")
                {
                    var s = JsonConvert.SerializeObject(o.Value);
                    status = JsonConvert.DeserializeObject<string>(s);
                }
                if (status == "true")
                {
                    if (o.Key == "user")
                    {
                        var s = JsonConvert.SerializeObject(o.Value);
                        userdata = JsonConvert.DeserializeObject<UserData>(s);
                    }
                    if (o.Key == "token")
                    {
                        var s = JsonConvert.SerializeObject(o.Value);
                        UserToken = JsonConvert.DeserializeObject<string>(s);
                    }
                }
                else
                {
                    break;
                }


            }

          

            _ = GoChatListPageAsync();


        }

    }
}
