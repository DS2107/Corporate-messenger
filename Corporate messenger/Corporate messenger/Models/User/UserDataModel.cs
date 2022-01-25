using Newtonsoft.Json;
using System.ComponentModel;
using Corporate_messenger.Models.Abstract;
using SQLite;

namespace Corporate_messenger.Models
{
    
    public class UserDataModel:UserAbstract, INotifyPropertyChanged
    {
        private static string last_login { get; set; }
        private static string created_at { get; set; }
        private static string updated_at { get; set; }      
        private static string avatar { get; set; }
        private static string active { get; set; }
        private static string email { get; set; }
        private static string token { get; set; }
        private static int id;
        private static string username;



        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// ID пользователя
        /// </summary>
        [PrimaryKey,Unique]
        [JsonProperty("id")]
        public override int Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        /// <summary>
        /// Token
        /// </summary>
        [JsonProperty("token")]
        public  string Token
        {
            get { return token; }
            set
            {
                if (token != value)
                {
                    token = value;
                    OnPropertyChanged("Token");
                }
            }
        }
        /// <summary>
        /// Имя пользователя пользователя
        /// </summary>
        [JsonProperty("username")]
        public override string Name
        {
            get { return username; }
            set
            {
                if (username != value)
                {
                    username = value;
                    OnPropertyChanged("Username");
                }
            }
        }
       
       
      

        

        /// <summary>
        /// Почта пользователя
        /// </summary>
        [JsonProperty("email")]
        public string Email
        {
            get { return email; }
            set
            {
                if (email != value)
                {
                    email = value;
                    OnPropertyChanged("Email");
                }
            }
        }

        /// <summary>
        /// Аватар пользователя
        /// </summary>
        [JsonProperty("avatar")]
        public string Avatar
        {
            get { return avatar; }
            set
            {
                if (avatar != value)
                {
                    avatar = value;
                    OnPropertyChanged("Avatar");
                }
            }
        }

        /// <summary>
        /// Активность в данный момент пользователя
        /// </summary>
        [JsonProperty("active")]
        public string Active
        {
            get { return active; }
            set
            {
                if (active != value)
                {
                    active = value;
                    OnPropertyChanged("Active");
                }
            }
        }

        /// <summary>
        /// Последняя авторизация пользователя
        /// </summary>
        [JsonProperty("last_login")]
        public string Last_login
        {
            get { return last_login; }
            set
            {
                if (last_login != value)
                {
                    last_login = value;
                    OnPropertyChanged("Last_login");
                }
            }
        }

        /// <summary>
        /// Дата создание записи 
        /// </summary>
        [JsonProperty("created_at")]
        public string Created_at
        {
            get { return created_at; }
            set
            {
                if (created_at != value)
                {
                    created_at = value;
                    OnPropertyChanged("Created_at");
                }
            }
        }

        /// <summary>
        /// Дата обновления записи
        /// </summary>
        [JsonProperty("updated_at")]
        public string Updated_at
        {
            get { return updated_at; }
            set
            {
                if (updated_at != value)
                {
                    updated_at = value;
                    OnPropertyChanged("Updated_at");
                }
            }
        }

       
    }
}
