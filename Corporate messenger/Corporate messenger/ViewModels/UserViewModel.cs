using Corporate_messenger.DB;
using Corporate_messenger.Models;
using Corporate_messenger.Models.Abstract;
using Corporate_messenger.Service;
using Corporate_messenger.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Corporate_messenger.ViewModels
{
    class UserViewModel:ApiAbstract, INotifyPropertyChanged
    {
        UserDataModel user;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        INavigation navigation;
        public UserViewModel(INavigation nav)
        {
            navigation=nav;
        }

        private string titleName { get; set; }

        private string name { get; set; }

        private string email { get; set; }

        private string token { get; set; }

        public string TitleName
        {
            get { return titleName; }
            set
            {
                if (titleName != value)
                {
                    titleName = value;
                    OnPropertyChanged("TitleName");
                }
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public string Email
        {
            get { return email; }
            set
            {
                if (email != value)
                {
                    email = value;
                    OnPropertyChanged("Email");
                }
            }
        }
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
        /// Команда для кнопки авторизации
        /// </summary>
        public ICommand ExitCommand
        {

            get
            {
                return new Command(async (object obj) =>
                {
                    user = await UserDbService.GetUser();
                    await UserDbService.RemoveUser(user.Id);
                    await ChatListDbService.DeleteAllChat();
                    await ChatDbService.DeleteAllMessage();
                   await navigation.PushAsync(new LoginPage());
                   // Application.Current.MainPage = DependencyService.Get<IFileService>().MyProperty;
                   // await Shell.Current.GoToAsync("//LoginPage",false);

                });
            }
        }

        public async Task GetInfoUser()
        {
            UserDataModel user = await UserDbService.GetUser();
            if (user != null)
            {
                TitleName = user.Name;
                Name = user.Name;
                Email = user.Email;
                Token = user.Token;
            }
           
            
        }

    }
}
