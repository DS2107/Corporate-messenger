using Corporate_messenger.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Corporate_messenger.ViewModels
{
    public class AuthorizationMainPageViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        
        public string userName { get; set; }
        public string UserName
        {
            get { return userName; }
            set
            {
                if (userName != value)
                {
                    userName = value;
                    OnPropertyChanged("UserName");
                }
            }
        }
        SpecialDataModel model = new SpecialDataModel();
        public AuthorizationMainPageViewModel()
        {
            
            UserName = model.Name;
        }


    }
}
