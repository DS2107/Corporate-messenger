using Corporate_messenger.DB;
using Corporate_messenger.Models;
using Corporate_messenger.Models.Abstract;
using Corporate_messenger.Service;
using Corporate_messenger.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Corporate_messenger.ViewModels
{
    public class LoginViewModel: ApiAbstract, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
       
        string MyEmail = "";
        string Pass = "";


        /// <summary>
        /// Список сообщений
        /// </summary>
        public string email
        {
            get { return MyEmail; }
            set
            {
                if (MyEmail != value)
                {
                    MyEmail = value;
                    OnPropertyChanged("email");
                }
            }
        }
        /// <summary>
        /// Список сообщений
        /// </summary>
        public string password
        {
            get { return Pass; }
            set
            {
                if (Pass != value)
                {
                    Pass = value;
                    OnPropertyChanged("email");
                }
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
                     await AuthorizationUserAsync();
                    
                });
            }
        }

        public bool status { get; set; }
        /// <summary>
        /// Метод для отправки данных о авторизации пользователя
        /// </summary>
        /// <returns></returns>
        private async Task AuthorizationUserAsync( )
        {           

            // Данные о пользователе которые пришли с сервера в случае удачной авторизации
            UserDataModel userdata = null;          
            //********** ЛОГИРОВАНИЕ **********
            // Перед отправкой , превращаем все в json
            string jsonLog = JsonConvert.SerializeObject(new { email,password});
            
            //****** РАСШИФРОВКА_ОТВЕТА ******

            JObject contentJobjects = await GetInfo_HttpMethod_Post_Async(jsonLog, "/api/login",true);
            if(contentJobjects == null)
            {
                DependencyService.Get<IFileService>().MyToast("Отсутствует соеденение с сервером, проверьте подключение к интернету и потворите попытку");
            }
            else
            {
                foreach (var KeyJobject in contentJobjects)
                {
                    if (KeyJobject.Key == "status")
                    {
                        var ValueJobject = JsonConvert.SerializeObject(KeyJobject.Value);
                        if (JsonConvert.DeserializeObject<string>(ValueJobject) == "true")
                            status = true;
                        else
                            status = false;

                    }
                    if (status)
                    {
                        if (KeyJobject.Key == "user")
                        {
                            var ValueJobject = JsonConvert.SerializeObject(KeyJobject.Value);
                            dynamic Json_obj = JObject.Parse(ValueJobject);
                            userdata = JsonConvert.DeserializeObject<UserDataModel>(ValueJobject);
                            
                      
                        }
                        if (KeyJobject.Key == "token")
                        {
                            var ValueJobject = JsonConvert.SerializeObject(KeyJobject.Value);                         
                            userdata.Token = JsonConvert.DeserializeObject<string>(ValueJobject);
                            await UserDbService.AddUser(userdata);
                           

                        }

                    }
                    else
                    {
                        break;
                    }
                }

                if (status != false)
                {
                   await Permission();
                    Application.Current.MainPage = new AuthorizationMainPage();
                    await Shell.Current.GoToAsync("//chats_list");    
                }
                else
                {
                    DependencyService.Get<IFileService>().MyToast("Неверный логин или пароль");
                }
            }
        }

    }
}
