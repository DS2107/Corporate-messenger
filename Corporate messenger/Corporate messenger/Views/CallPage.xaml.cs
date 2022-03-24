using Android.Media;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Net.Sip;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Corporate_messenger.Service;
using Corporate_messenger.ViewModels;

namespace Corporate_messenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CallPage : ContentPage
    {
      
        public CallPage(bool init_call)
        {
            InitializeComponent();
            BindingContext = new CallViewModel(Navigation,init_call);
            Shell.SetTabBarIsVisible(this, false);
            //  CallClass call = new CallClass();
            //  call.LessPort();
        }

        public void SetName(string name)
        {
            User_Name_Label.Text = name;

        }


    }
}