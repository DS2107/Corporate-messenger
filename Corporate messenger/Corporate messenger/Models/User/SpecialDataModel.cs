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

        private static int input_chat { get; set; }

        private static int id { get; set; }

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
        /// Комната чата
        /// </summary>   
        public int Input_chat
        {
            get { return input_chat; }
            set
            {
                if (input_chat != value)
                {
                    input_chat = value;
                    OnPropertyChanged("Input_chat");
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


        /// <summary>
        /// Токен пользователя
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
