using Corporate_messenger.Models;
using Corporate_messenger.Service;
using Corporate_messenger.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Corporate_messenger.ViewModels
{
    public class LoginViewModel
    {
        /// <summary>
        /// Команда для кнопки авторизации
        /// </summary>
        public ICommand AuthorizationUserCommand { get; set; }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public LoginViewModel()
        {
            _ = Permission();
            // Регестрирую команду для кнопки на LoginPage
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


        async Task Permission()
        {
            // Даю разрешения для микрофона и (зписи/чтения файлов)
            var PermissionsStrorage_Write = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            // Даю разрешения для микрофона и (зписи/чтения файлов)
            var PermissionsStrorage_Read = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            // Прверка разрешенний
            if (PermissionsStrorage_Write != PermissionStatus.Granted && PermissionsStrorage_Read != PermissionStatus.Granted)
            {
                PermissionsStrorage_Write = await Permissions.RequestAsync<Permissions.StorageWrite>();
                PermissionsStrorage_Read = await Permissions.RequestAsync<Permissions.StorageRead>();
            }
            if (PermissionsStrorage_Write != PermissionStatus.Granted && PermissionsStrorage_Read != PermissionStatus.Granted)
            {
                return;
            }
        }
        
        private void AuthorizationUserAsync(object obj)
        {
            _ = AuthorizationUserAsync();
        }

        /// <summary>
        /// Метод для отправки данных о авторизации пользователя
        /// </summary>
        /// <returns></returns>
        private async Task AuthorizationUserAsync( )
        {
            
            // Модель авторизации
            LoginModel log = new LoginModel();

            // Модель спец данных
            SpecialDataModel specialData = new SpecialDataModel();

            // Данные о пользователе которые пришли с сервера в случае удачной авторизации
            UserDataModel userdata = null;
           

            //********** ЛОГИРОВАНИЕ **********
            // Перед отправкой , превращаем все в json
            string jsonLog = JsonConvert.SerializeObject(log);
            
            // Устанавливаем соеденение 
            HttpClient client = new HttpClient();

            // Тип данных который мы принимаем от сервера 
            var contentType = "application/json"; 

            // Тип Запроса
            var httpMethod = HttpMethod.Post; 
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri("http://192.168.0.105:8098/api/login"),
                Method = httpMethod,
                Content = new StringContent(jsonLog, System.Text.Encoding.UTF8, contentType)
            };
            // Отправка данных авторизации
            var httpResponse = await client.SendAsync(request);

            // Ответ от сервера 
            var contenJSON = await httpResponse.Content.ReadAsStringAsync();

            //****** РАСШИФРОВКА_ОТВЕТА ******
            JObject contentJobjects = JObject.Parse(contenJSON);
    
            foreach (var KeyJobject in contentJobjects)
            {
                if (KeyJobject.Key == "status")
                {
                    var ValueJobject = JsonConvert.SerializeObject(KeyJobject.Value);
                    if (JsonConvert.DeserializeObject<string>(ValueJobject) == "true")
                        specialData.Status = true;
                    else
                        specialData.Status = false;
                 
                }
                if (specialData.Status)
                {
                    if (KeyJobject.Key == "user")
                    {
                        var ValueJobject = JsonConvert.SerializeObject(KeyJobject.Value);
                        userdata = JsonConvert.DeserializeObject<UserDataModel>(ValueJobject);
                        specialData.Id = userdata.Id;
                    }
                    if (KeyJobject.Key == "token")
                    {
                        var ValueJobject = JsonConvert.SerializeObject(KeyJobject.Value);
                        specialData.Token = JsonConvert.DeserializeObject<string>(ValueJobject);
                        DependencyService.Get<IFileService>().CreateFile(specialData.Token,specialData.Id);

                    }
                }
                else
                {
                    break;
                }


            }
            
          if(specialData.Token != "" || specialData.Status)
            _ = GoChatListPageAsync();
          
                

        }

    }
}
