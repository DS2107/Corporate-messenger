using Corporate_messenger.DB;
using Corporate_messenger.Models.Chat;
using Corporate_messenger.Service;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Corporate_messenger.Models.Abstract
{
    public abstract class ApiAbstract
    {

        UserDataModel model;
       
        
        public static string addressWS = "ws://"+"192.168.10.254:55201";   
      
      
        // Устанавливаем соеденение 
        HttpClient client;
        
        /// <summary>
        /// Модель данных чат
        /// </summary>
        public ChatModel chat = new ChatModel();
        public async Task<JObject> GetInfo_HttpMethod_Get_Async(string url)
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(20);
            model = await UserDbService.GetUser();
            try
            {
                // Тип Запроса
                var httpMethod = HttpMethod.Get;
                var address = "http://192.168.10.254:55200" + url;

                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(address),
                    Method = httpMethod,
                };

                // Отправка заголовка
                request.Headers.Add("Authorization", "Bearer " + model.Token);

                // Отправка данных 
                var httpResponse = await client.SendAsync(request);

                // Ответ от сервера 
                var contenJSON = await httpResponse.Content.ReadAsStringAsync();

                //****** РАСШИФРОВКА_ОТВЕТА ******
                JObject contentJobjects = JObject.Parse(contenJSON);

                return contentJobjects;
            }
            catch (Exception ex)
            {
                var ext = ex;
                return null;
            }
            
        }
        public async Task<JObject> GetInfo_HttpMethod_Post_Async(string jsonLog,string url,bool login)
        {
            client = new HttpClient();
            model = await UserDbService.GetUser();
            try
            {
                // Тип данных который мы принимаем от сервера 
                var contentType = "application/json";
                // Тип Запроса
                var httpMethod = HttpMethod.Post;
                var address = "http://192.168.10.254:55200" + url;
                // StringContent? conten = new StringContent(jsonLog, System.Text.Encoding.UTF8, contentType);
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(address),
                    Method = httpMethod,
                    Content = new StringContent(jsonLog, System.Text.Encoding.UTF8, contentType)
                };
                // Отправка заголовка
                request.Headers.Add("Accept", "application/json");
                if(login == false)
                    request.Headers.Add("Authorization", "Bearer " + model.Token);
                // Отправка данных авторизации
                var httpResponse = await client.SendAsync(request);
                // Ответ от сервера 
                var contenJSON = await httpResponse.Content.ReadAsStringAsync();

                //****** РАСШИФРОВКА_ОТВЕТА ******
                JObject contentJobjects = JObject.Parse(contenJSON);
                // Ответ от сервера 
                return contentJobjects;
            }
            catch(Exception ex)
            {
                var ext = ex;
                return null;
            }
           
        }
        /// <summary>
        /// Разрешения
        /// </summary>
        /// <returns></returns>
       public async Task Permission()
        {
            // Даю разрешения для микрофона и (зписи/чтения файлов)
            var PermissionsStrorage_Write = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();


            var PermissionsStrorage_Read = await Permissions.CheckStatusAsync<Permissions.StorageRead>();

            var PermissionsRecord = await Permissions.CheckStatusAsync<Permissions.Microphone>();
            // Прверка разрешенний
            if (PermissionsRecord != PermissionStatus.Granted)
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
    }
}
