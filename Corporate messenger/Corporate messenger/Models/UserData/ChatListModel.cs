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
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        private  int id { get; set; }
        private string title { get; set; }
        private string last_message { get; set; }

        /// <summary>
        /// ID пользователя 
        /// </summary>
        [JsonProperty("id")]
        [PrimaryKey, AutoIncrement, Column("_id")]
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
            get { return title; }
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
            get { return last_message; }
            set
            {
                if (last_message != value)
                {
                    last_message = value;
                    OnPropertyChanged("Last_message");
                }
            }
        }

    }
}
