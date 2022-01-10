using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Corporate_messenger.Models.UserData
{
    class FriendsModel : UserAbstract, INotifyPropertyChanged
    {
        private string last_login { get; set; }

        private string username;

        private string email { get; set; }

        //private string avatar { get; set; }

        private int id;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


        /// <summary>
        /// ID друга
        /// </summary>
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
        /// Имя друга
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

       
    }
}
