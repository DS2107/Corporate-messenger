using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Corporate_messenger.Models
{
    public class SpecialDataModel : UserAbstract, INotifyPropertyChanged
    {
        private static int input_chat { get; set; }
        private static string token { get; set; }     
        private static bool status { get; set; }
        private int receiver_id { get; set; }
        private static int id;
        private static string name;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        
       
        // <summary>
        /// id usera
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
        /// Имя пользователя который авторизовался
        /// </summary>
        public override string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged("name");
                }
            }
        }
      
      

        /// <summary>
        /// id пользователя
        /// </summary>
        [JsonProperty("receiverId")]
        public int receiverId
        {
            get { return receiver_id; }
            set
            {
                if (receiver_id != value)
                {
                    receiver_id = value;
                    OnPropertyChanged("Sender_id");
                }
            }
        }

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


       

       
    }
}
