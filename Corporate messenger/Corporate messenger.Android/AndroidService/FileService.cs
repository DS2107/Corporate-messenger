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
        public string CreateAudioFile()
        {
            string filename = Path.Combine(GetRootPath(), "audio.amr");

            if(File.Exists(filename))
            {
                File.Delete(filename);
            }
           

            return  Path.Combine(GetRootPath(), filename);

        }

        public string GetAudioFile()
        {
            string filename = Path.Combine(GetRootPath(), "audio.amr");
            return Path.Combine(GetRootPath(), filename);
        }
        
        public string SaveFile(byte[] audio)
        {
            string filename = Path.Combine(GetRootPath(), "audio2.amr");
            File.WriteAllBytes(filename, audio);
            return filename;

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