using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Corporate_messenger.Models;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http.Formatting;
using Newtonsoft.Json.Linq;

namespace Corporate_messenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        
      
        public LoginPage()
        {
            InitializeComponent();
           
        }

        private async void B_login_Clicked(object sender, EventArgs e)
        {
            var LogUser = new LogUser();
            LogUser.email = UsernameTxt.Text;
            LogUser.password = Pass.Text;
            string UserToken="";

            //ЛОГИРОВАНИЕ**********
            string jsonLog = JsonConvert.SerializeObject(LogUser);
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
               if(o.Key == "status")
                {
                    var s = JsonConvert.SerializeObject(o.Value);
                    status = JsonConvert.DeserializeObject<string>(s);                   
                }
               if(status == "true")
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
           
            //dynamic report = JsonConvert.DeserializeObject(contenJSON);



            await Shell.Current.GoToAsync("//chats_list",true);
            string value = UsernameTxt.Text;
            UsernameTxt.Text = value;
            Preferences.Set("name", value);
        }

        protected override void OnAppearing()
        {
            string name = Preferences.Get("name", "Username");
            UsernameTxt.Text = name;
            base.OnAppearing();
           
        }
    }
}