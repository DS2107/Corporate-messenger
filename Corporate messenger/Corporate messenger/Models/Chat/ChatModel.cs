using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Corporate_messenger.Models.Chat
{
    class ChatModel: INotifyPropertyChanged
    {
        private int sender_id { get; set; }
        private string message { get; set; }
        private int chat_room_id { get; set; }


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
