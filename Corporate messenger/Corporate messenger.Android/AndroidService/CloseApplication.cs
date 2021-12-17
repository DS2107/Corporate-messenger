using Android.App;
using Corporate_messenger.Droid.AndroidService;
using Corporate_messenger.Service;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(CloseApplication))]
namespace Corporate_messenger.Droid.AndroidService
{
    class CloseApplication:ICloseApplication
    {

        public void closeApplication()
        {
            var activity = (Activity)Forms.Context;
            activity.FinishAffinity();
        }
    }
}