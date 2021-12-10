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

        private INavigation Nav { get; set; }
        /// <summary>
        /// Конструктор класса
        /// </summary>
        public LoginViewModel(INavigation navigation)
        {
            _ = Permission();                  
            Nav = navigation;
        }

        /// <summary>
        /// Переход в приложения 
        /// </summary>
        /// <returns></returns>
        async Task GoChatListPageAsync()
        {
          
            await Application.Current.MainPage.Navigation.PopAsync();
            Application.Current.MainPage = new AuthorizationMainPage();
          
          
        }


        async Task Permission()
        {
            // Даю разрешения для микрофона и (зписи/чтения файлов)
            var PermissionsStrorage_Write = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

            var PermissionsStrorage_Read = await Permissions.CheckStatusAsync<Permissions.StorageRead>();

            var PermissionsRecord = await Permissions.CheckStatusAsync<Permissions.Microphone>();
            // Прверка разрешенний
            if (PermissionsRecord != PermissionStatus.Granted )
            {
                PermissionsRecord = await Permissions.RequestAsync<Permissions.Microphone>();
              
            }
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
        /// <summary>
        /// Команда для кнопки авторизации
        /// </summary>
        public ICommand AuthorizationUserCommand {

            get
            {
                return new Command(async (object obj) =>
                {
                    
                     AuthorizationUserAsync();
                });
            }
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
            var address = DependencyService.Get<IFileService>().CreateFile() + "/api/login";
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(address),
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
                        specialData.Name = userdata.Username;
                    }
                    if (KeyJobject.Key == "token")
                    {
                        var ValueJobject = JsonConvert.SerializeObject(KeyJobject.Value);
                        specialData.Token = JsonConvert.DeserializeObject<string>(ValueJobject);
                        DependencyService.Get<IFileService>().CreateFile(specialData.Token, specialData.Id, specialData.Name);

                    }
                    
                }
                else
                {
                    break;
                }
            }

            if (specialData.Status != false)
            {
                await  GoChatListPageAsync();
            }
            else
            {
                DependencyService.Get<IFileService>().MyToast();
            }
           
          
                

        }

    }
}
