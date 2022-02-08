using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Corporate_messenger.Service
{
    public interface IFileService
    {
        public Page MyMainPage { get; set; }

        public string GetPath(string name);
        public bool flag { get; set; }
        /// <summary>
        /// Получить корневую папку проекта
        /// </summary>
        /// <returns></returns>
        string GetRootPath();

        /// <summary>
        /// Создать файл Config
        /// </summary>
        /// <returns></returns>
        string CreateFile();
        public string GetDb();
        public void CreateDb();

        /// <summary>
        /// Создать файл Token
        /// </summary>
        /// <param name="token">Токен юзера</param>
        /// <param name="userID">ID юзера</param>
        /// <param name="name">Его имя</param>
        void CreateFile(string token,int userID,string name);

        /// <summary>
        /// создать аудио файл
        /// </summary>
        /// <returns></returns>
        string CreateAudioFile();

        /// <summary>
        /// Получить аудио фалй
        /// </summary>
        /// <returns></returns>
        string GetAudioFile();

        /// <summary>
        /// Сохранить файл
        /// </summary>
        /// <param name="audio"></param>
        /// <returns></returns>
        string SaveFile(byte[] audio);


        /// <summary>
        /// Чтение файла
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        string ReadFile(string file);


        /// <summary>
        /// Удалить файл
        /// </summary>
        void Delete();

        /// <summary>
        /// Вызвать оповещание
        /// </summary>
        void MyToast(string text);

       

    }
}
