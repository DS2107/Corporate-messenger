using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Corporate_messenger.Models
{
    [Table("ChatList")]
    public class ChatListModel: INotifyPropertyChanged
    {
        private string last_message { get; set; }
        private string title { get; set; }    
        private  int id { get; set; }
        private string updated_at { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// ID пользователя 
        /// </summary>
        [JsonProperty("id")]
        [PrimaryKey]
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
        /// Название беседы
        /// </summary>
        [JsonProperty("title")]
        public string Title
        {
            get {return title; }
            set
            {
                if (title != value)
                {
                    title = value;
                    OnPropertyChanged("title");
                }
            }
        }

        /// <summary>
        /// Последнее сообщение
        /// </summary>
        [JsonProperty("last_message")]
        public string Last_message
        {
            get {
                if (last_message.Length > 35)
                {
                    last_message = last_message.Substring(0, 35) + "...";
                    return last_message;
                }
                else
                {
                    return last_message;
                }

                }
            set
            {
                if (last_message != value)
                {
                    last_message = value;
                    OnPropertyChanged("Last_message");
                }
            }
        }

        /// <summary>
        /// время изменения чата 
        /// </summary>
        [JsonProperty("updated_at")]
        public string Updated_at
        {
            get
            {    return updated_at;

            }
            set
            {
                if (updated_at != value)
                {
                    updated_at = value;
                    OnPropertyChanged("Updated_at");
                }
            }
        }

    }
}
