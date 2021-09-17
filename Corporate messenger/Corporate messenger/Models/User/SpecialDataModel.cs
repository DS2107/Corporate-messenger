using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Corporate_messenger.Models
{
    class SpecialDataModel: INotifyPropertyChanged
    {
        private static bool status { get; set; }

        private static string  token { get; set; }

        /// <summary>
        /// Статус пользователя
        /// </summary>
        [JsonProperty("status")]
        public bool Status
        {
            get { return status; }
            set
            {
                if (status != value)
                {
                    status = value;
                    OnPropertyChanged("Status");
                }
            }
        }
        /// <summary>
        /// Токен пользователя
        /// </summary>
        [JsonProperty("token")]
        public string Token
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
