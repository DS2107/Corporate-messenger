﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Corporate_messenger.Models
{
    class LoginModel:INotifyPropertyChanged
    {
        
        private static string email { get; set; }

        private static string password { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
      
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        /// <summary>
        /// Логин пользователя
        /// </summary>
        [JsonProperty("email")]
        public string Login
        {
            get { return email; }
            set
            {
                if (email != value)
                {
                    email = value;
                    OnPropertyChanged("Login");
                }
            }
        }
        /// <summary>
        /// Пароль пользователя
        /// </summary>
        [JsonProperty("password")]
        public string Password
        {
            get { return password; }
            set
            {
                if (password != value)
                {
                    password = value;
                    OnPropertyChanged("Password");
                }
            }
        }
    }
}