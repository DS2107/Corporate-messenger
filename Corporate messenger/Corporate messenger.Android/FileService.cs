using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Corporate_messenger.Droid;
using Corporate_messenger.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(FileService))]
namespace Corporate_messenger.Droid
{
    class FileService : IFileService
    {
        public string GetRootPath()
        {
            return Application.Context.GetExternalFilesDir(null).ToString();
        }

        public void CreateFile(string token,int userID)
        {
            var filename = "token.txt";

            var destination = Path.Combine(GetRootPath(), filename);

            File.WriteAllText(destination, token + "/" + userID);
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
    }
}