using Corporate_messenger.Models.Abstract;
using Corporate_messenger.Service;
using Corporate_messenger.Service.Notification;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Corporate_messenger.ViewModels
{
    class MainPageViewModel: ApiAbstract, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

       
        public string Name
        {
            get { return SpecDataUser.Name; }
            set
            {
                if (SpecDataUser.Name != value)
                {
                    SpecDataUser.Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        public MainPageViewModel()
        {
          //  if (DependencyService.Get<IForegroundService>().SocketFlag == false)
           //     DependencyService.Get<IForegroundService>().StartService();
        }

        public ICommand GoLogin
        {

            get
            {
                return new Command(async() =>
                {
                  await Shell.Current.GoToAsync("//login");
                });

            }
        }


    }
}
