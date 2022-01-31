using Corporate_messenger.Models;
using Corporate_messenger.Models.Chat;
using Corporate_messenger.Service;
using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Corporate_messenger.DB
{
   public class UserDbService
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
                await db.CreateTableAsync<UserDataModel>();
                await db.CreateTableAsync<ChatListModel>();
                await db.CreateTableAsync<ChatModel>();
            }
                

           
        }

        public static async Task AddUser(UserDataModel data)
        {
            await Init();
            UserDataModel user = data;

            await db.InsertAsync(user);
        }

        public static async Task RemoveUser(int id)
        {
            await Init();
            await db.DeleteAsync<UserDataModel>(id);
        }

        public static async Task<UserDataModel> GetUser()
        {
            await Init();
            var user = await db.Table<UserDataModel>().FirstOrDefaultAsync();
            return user;
        }
      

    }
}
