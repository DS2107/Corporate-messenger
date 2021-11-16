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
        private byte[] audio { get; set; }
        private string type { get; set; }
        private int receiver_id { get; set; }
        private string sourceImage { get; set; }

        private double valueSlider { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


       

        private bool isEnableSlider { get; set; }

        public bool IsEnableSlider
        {
            get { return isEnableSlider; }
            set
            {
                if (isEnableSlider != value)
                {
                    isEnableSlider = value;
                    OnPropertyChanged("IsEnableSlider");
                }
            }
        }

        private bool isMessageVisible { get; set; }

        public bool IsMessageVisible
        {
            get { return isMessageVisible; }
            set
            {
                if (isMessageVisible != value)
                {
                    isMessageVisible = value;
                    OnPropertyChanged("IsMessageVisible");
                }
            }
        }

        private bool isAuidoVisible { get; set; }

        public bool IsAuidoVisible
        {
            get { return isAuidoVisible; }
            set
            {
                if (isAuidoVisible != value)
                {
                    isAuidoVisible = value;
                    OnPropertyChanged("IsAuidoVisible");
                }
            }
        }

        private double maximumSlider { get; set; }
        public double ValueSlider
        {
            get { return valueSlider; }
            set
            {
               
                if (valueSlider != value)
                {
                    valueSlider = value;
                    OnPropertyChanged("ValueSlider");
                }
            }
        }

        public double MaximumSlider
        {
            get { return maximumSlider; }
            set
            {
                
                if (maximumSlider != value)
                {
                    maximumSlider = value;
                    OnPropertyChanged("MaximumSlider");
                }
            }
        }


        public string SourceImage
        {
            get { return sourceImage; }
            set
            {
                if (sourceImage != value)
                {
                    sourceImage = value;
                    OnPropertyChanged("SourceImage");
                }
            }
        }

        /// <summary>
        /// Аудио сообщение
        /// </summary>
        [JsonProperty("audio")]
        public byte[] Audio
        {
            get { return audio; }
            set
            {
                if (audio != value)
                {
                    audio = value;
                    OnPropertyChanged("Audio");
                }
            }
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
