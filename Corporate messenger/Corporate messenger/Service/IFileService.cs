using System;
using System.Collections.Generic;
using System.Text;

namespace Corporate_messenger.Service
{
    public interface IFileService
    {
        string GetRootPath();
        void CreateFile(string token,int userID);

        string ReadFile(string file);

        void Delete();


    }
}
