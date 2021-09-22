using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Corporate_messenger.Models;
using SQLite;

namespace Corporate_messenger.DB.Repository
{
    public class ChatListRepository
    {
        // Класс который предоставляет методы работы с бд
        SQLiteConnection database;

        /// <summary>
        /// Констркутор для создания 
        /// </summary>
        /// <param name="databasePath">Путь к бд</param>
        public ChatListRepository(string databasePath)
        {
           
            database = new SQLiteConnection(databasePath);
           
            database.CreateTable<ChatListModel>();
        }
        public void DropTable()
        {
            database.DropTable<ChatListModel>();
        }
        /// <summary>
        /// Вернуть все элементы
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<ChatListModel> GetItems()
        {
            ObservableCollection<ChatListModel> collection = new ObservableCollection<ChatListModel>(database.Table<ChatListModel>().ToList());
            return collection;
        }

        /// <summary>
        /// Вернуть орпеделенный элемент
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ChatListModel GetItem(int id)
        {
            return database.Get<ChatListModel>(id);
        }

        /// <summary>
        /// Удалить элемент
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteItem(int id)
        {
            return database.Delete<ChatListModel>(id);
        }

        /// <summary>
        /// Сохранить элемент
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int SaveItem(ChatListModel item)
        {
            if (item.Id != 0)
            {
                database.Update(item);
                return item.Id;
            }
            else
            {
                return database.Insert(item);
            }
        }


       
    }
}
