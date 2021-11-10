using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Corporate_messenger.Service
{
    public interface IFileService
    {
        string GetRootPath();
        void CreateFile(string token,int userID,string name);

        string ReadFile(string file);

        void Delete();

        void MyToast();

       

    }
}
