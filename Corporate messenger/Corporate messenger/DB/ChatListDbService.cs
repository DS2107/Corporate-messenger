using SQLite;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Corporate_messenger.Service;
using Corporate_messenger.Models;

namespace Corporate_messenger.DB
{
    class ChatListDbService
    {
        static SQLiteAsyncConnection db;
        static async Task Init()
        {
           

            if (File.Exists(DependencyService.Get<IFileService>().GetDb()))
            {
                if (db != null)
                    return;

                db = new SQLiteAsyncConnection(DependencyService.Get<IFileService>().GetDb());


            }
            else
            {
                DependencyService.Get<IFileService>().CreateDb();
                db = new SQLiteAsyncConnection(DependencyService.Get<IFileService>().GetDb());
                await db.CreateTableAsync<ChatListModel>();
            }
        }

        public static async Task AddChat(ChatListModel data)
        {
            await Init();
            ChatListModel chat = data;

            await db.InsertAsync(chat);
        }

        public static async Task RemoveChat(int id)
        {
            await Init();
            await db.DeleteAsync<ChatListModel>(id);
        }

        public static async Task<ChatListModel> Getchat()
        {
            await Init();
            var user = await db.Table<ChatListModel>().FirstOrDefaultAsync();
            return user;
        }
    }
}
