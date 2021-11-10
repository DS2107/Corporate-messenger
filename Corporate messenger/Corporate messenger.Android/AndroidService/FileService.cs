using Android.App;
using Android.Widget;
using Corporate_messenger.Droid;
using Corporate_messenger.Service;
using System.IO;


[assembly: Xamarin.Forms.Dependency(typeof(FileService))]
namespace Corporate_messenger.Droid
{
    class FileService : IFileService
    {
        public string GetRootPath()
        {
            return Application.Context.GetExternalFilesDir(null).ToString();
        }

        public void CreateFile(string token,int userID,string name)
        {
            var filename = "token.txt";

            var destination = Path.Combine(GetRootPath(), filename);

            File.WriteAllText(destination, token + "/" + userID + "/" + name);
        }

        public string ReadFile(string file)
        {
            file = file + "/token.txt";
            if (File.Exists(file))
            {
                return File.ReadAllText(file);
            }
            else
            {
                return "";
            }
           
        }

        public void Delete()
        {
            var filename = "token.txt";

            var destination = Path.Combine(GetRootPath(), filename);

            File.Delete(destination);
        }


        public void MyToast()
        {
            Toast.MakeText(Application.Context, "Неправильный пароль или логин", ToastLength.Short).Show();
        }

        
    }
}