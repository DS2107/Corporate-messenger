﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Corporate_messenger.Service
{
    public interface IFileService
    {
        string GetRootPath();
        string CreateFile();
        void CreateFile(string token,int userID,string name);
        string CreateAudioFile();
        string GetAudioFile();

        string SaveFile(byte[] audio);
        string ReadFile(string file);

        void Delete();

        void MyToast();

       

    }
}
