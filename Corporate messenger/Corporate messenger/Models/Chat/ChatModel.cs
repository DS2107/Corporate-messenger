using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Corporate_messenger.Models.Chat
{
    class ChatModel: INotifyPropertyChanged
    {
        private  int sender_id { get; set; }
        private string message { get; set; }
        private int chat_room_id { get; set; }
        private string time_LstMessage { get; set; } 
   
        private string type { get; set; }
        private int receiver_id { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// Логин пользователя
        /// </summary>
        [JsonProperty("sender_id")]
        public int Sender_id
        {
            get { return sender_id; }
            set
            {
                if (sender_id != value)
                {
                    sender_id = value;
                    OnPropertyChanged("Sender_id");
                }
            }
        }


        /// <summary>
        /// Время ласт сообщения
        /// </summary>
        [JsonProperty("created_at")]
        public string Time_LstMessage
        {
            
            get {
               
                return time_LstMessage; }
            set
            {
                if (time_LstMessage != value)
                {
                    time_LstMessage = value;
                    OnPropertyChanged("Time_LstMessage");
                }
            }
        }


        /// <summary>
        /// ID Отправителя
        /// </summary>
        [JsonProperty("receiverId")]
        public int Receiver_id
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
        /// Логин пользователя
        /// </summary>
        [JsonProperty("type")]
        public string TypeMessage
        {
            get { return type; }
            set
            {
                if (type != value)
                {
                    type = value;
                    OnPropertyChanged("TypeMessage");
                }
            }
        }

        /// <summary>
        /// Логин пользователя
        /// </summary>
        [JsonProperty("message")]
        public string Message
        {
            get { return message; }
            set
            {
                if (message != value)
                {
                    message = value;
                    OnPropertyChanged("Message");
                }
            }
        }

        /// <summary>
        /// Логин пользователя
        /// </summary>
        [JsonProperty("chat_room_id")]
        public int Chat_room_id
        {
            get { return chat_room_id; }
            set
            {
                if (chat_room_id != value)
                {
                    chat_room_id = value;
                    OnPropertyChanged("Chat_room_id");
                }
            }
        }
    }
}
