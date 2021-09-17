using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Corporate_messenger.Models
{
    public class LogUser:INotifyPropertyChanged
    {
        public static string email { get; set; }

        public static string password { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
      
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

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
        [JsonProperty("password")]
        public string Pass
        {
            get { return password; }
            set
            {
                if (password != value)
                {
                    password = value;
                    OnPropertyChanged("Pass");
                }
            }
        }
    }
}
