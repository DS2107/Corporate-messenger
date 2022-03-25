using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Corporate_messenger.Models.UserData
{
    class CallListModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        string title;
        string time;
        string icon_type;
        int type_call;
        int id_titleUser;


        /// <summary>
        /// ID пользователя 
        /// </summary>
        [JsonProperty("id")]
        [PrimaryKey]
        public int Id_titleUser
        {
            get { return id_titleUser; }
            set
            {
                if (id_titleUser != value)
                {
                    id_titleUser = value;
                    OnPropertyChanged("Id_titleUser");
                }
            }
        }

        [JsonProperty("id")]      
        public int Type_call
        {
            get { return type_call; }
            set
            {
                if (type_call != value)
                {
                    type_call = value;
                    OnPropertyChanged("Type_call");
                }
            }
        }

        [JsonProperty("id")]     
        public string Icon_type
        {
            get { return icon_type; }
            set
            {
                if (icon_type != value)
                {
                    icon_type = value;
                    OnPropertyChanged("Icon_type");
                }
            }
        }

        [JsonProperty("id")]
        public string Time
        {
            get { return time; }
            set
            {
                if (time != value)
                {
                    time = value;
                    OnPropertyChanged("Time");
                }
            }
        }

        [JsonProperty("id")]
        public string Title
        {
            get { return title; }
            set
            {
                if (title != value)
                {
                    title = value;
                    OnPropertyChanged("Title");
                }
            }
        }


    }
}
