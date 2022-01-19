using Corporate_messenger.Models;
using Corporate_messenger.Models.Abstract;
using Corporate_messenger.Service;
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
        private INavigation Nav { get; set; }
        /// <summary>
        /// Конструктор класса
        /// </summary>
        public LoginViewModel(INavigation navigation)
        {
            _ = Permission();                  
            Nav = navigation;
           // Application.Current.MainPage = new LoginPage();
        }

        private  void Autorize()
        {
            Application.Current.MainPage = DependencyService.Get<IFileService>().MyProperty;

        }

        /// <summary>
        /// Разрешения
        /// </summary>
        /// <returns></returns>
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
                     await AuthorizationUserAsync();
                    
                });
            }
        }
     

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

            JObject contentJobjects = await GetInfo_HttpMethod_Post_Async(jsonLog, "/api/login");
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
                            SpecDataUser.Status = true;
                        else
                            SpecDataUser.Status = false;

                    }
                    if (SpecDataUser.Status)
                    {
                        if (KeyJobject.Key == "user")
                        {
                            var ValueJobject = JsonConvert.SerializeObject(KeyJobject.Value);
                            dynamic Json_obj = JObject.Parse(ValueJobject);
                            userdata = JsonConvert.DeserializeObject<UserDataModel>(ValueJobject);

                            SpecDataUser.Id = (int)Json_obj.id;
                            SpecDataUser.Name = (string)Json_obj.username;
                        }
                        if (KeyJobject.Key == "token")
                        {
                            var ValueJobject = JsonConvert.SerializeObject(KeyJobject.Value);
                            SpecDataUser.Token = JsonConvert.DeserializeObject<string>(ValueJobject);
                            DependencyService.Get<IFileService>().CreateFile(SpecDataUser.Token, SpecDataUser.Id, SpecDataUser.Name);

                        }

                    }
                    else
                    {
                        break;
                    }
                }

                if (SpecDataUser.Status != false)
                {
                    Autorize();
                    await Shell.Current.GoToAsync($"//chats_list");
                }
                else
                {
                    DependencyService.Get<IFileService>().MyToast("Неверный логин или пароль");
                }
            }
        }

    }
}
