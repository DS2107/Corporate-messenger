using Corporate_messenger.DB;
using Corporate_messenger.Models;
using Corporate_messenger.Models.UserData;
using Corporate_messenger.Service;
using Corporate_messenger.Service.Notification;
using Corporate_messenger.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Corporate_messenger.ViewModels
{
    class CallListViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public ObservableCollection<CallListModel> CallList
        {
            get { return callList; }
            set
            {
                if (callList != value)
                {
                    callList = value;
                    OnPropertyChanged("CallListModel");
                }
            }
        }
        private ObservableCollection<CallListModel> callList = new ObservableCollection<CallListModel>();
        public ICommand Call
        {
            get
            {
                return new Command(async (object obj) => {
                    // Ищем нужный элемент
                    if (obj is CallListModel item)
                    {



                        MyUser = await UserDbService.GetUser();
                        CallPage callPage = new CallPage(true);
                        callPage.SetName(item.Title);
                        await Application.Current.MainPage.Navigation.PushAsync(callPage);
                        try
                        {
                            DependencyService.Get<IAudioUDPSocketCall>().ConnectionToServer();
                            var s = DependencyService.Get<IAudioUDPSocketCall>().GetServerIp();
                            DependencyService.Get<ISocket>().MyWebSocket.Send(JsonConvert.SerializeObject(new
                            {
                                type = "init_call",
                                status = "100",
                                sender_id = MyUser.Id,
                                receiver_id = item.Id_titleUser,
                                call_address = DependencyService.Get<IAudioUDPSocketCall>().GetServerIp()
                            }));
                            DependencyService.Get<IAudio>().PlayAudioFile("gudok.mp3", Android.Media.Stream.VoiceCall);
                            DependencyService.Get<IForegroundService>().Flag_AudioCalls_Init = true;


                        }
                        catch (Exception ex)
                        {
                            DependencyService.Get<IForegroundService>().MyToast("Не удается позвонить, возможно потеряно соединение с сервором: " + ex.Message);
                        }

                    }
                });
            }
        }
        UserDataModel MyUser;
     
        

        public CallListViewModel()
        {

        }
    }
}
