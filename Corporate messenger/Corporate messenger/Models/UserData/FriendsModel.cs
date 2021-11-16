using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Corporate_messenger.Models.UserData
{
    class FriendsModel: INotifyPropertyChanged
    {
        private string username { get; set; }

        private string email { get; set; }

        //private string avatar { get; set; }

        private int id { get; set; }

        private string last_login { get; set; }


        /// <summary>
        /// ID друга
        /// </summary>
        [JsonProperty("id")]
        public int Id
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
        /// Имя друга
        /// </summary>
        [JsonProperty("username")]
        public string Username
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
        /// Почта друга
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

      /*  /// <summary>
        /// Аватар друга
        /// </summary>
        [JsonProperty("id")]
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
        }*/

        /// <summary>
        /// Последний раз когда заходил друг
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
