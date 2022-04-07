using Corporate_messenger.Models.Chat;
using Corporate_messenger.Service;
using SQLite;
using System.Linq;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using System;

namespace Corporate_messenger.DB
{
    class ChatDbService
    {
        static SQLiteAsyncConnection db;


        static void Init()
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

            }
        }

        public static async Task AddMessage(ChatModel data)
        {
            Init();
            ChatModel chat = data;

            await db.InsertAsync(chat);
        }

        public static async Task RemoveMessage(int id)
        {
            Init();
            await db.DeleteAsync<ChatModel>(id);
        }
        public static async Task<ObservableCollection<ChatModel>> GetLastMessages(int room_id)
        {
            Init();
            string query = "SELECT* FROM ChatModel WHERE chat_room_id=" + room_id + " ORDER BY message_id DESC LIMIT 10";
            var messages = await db.QueryAsync<ChatModel>(query);
            var myObservableCollection = new ObservableCollection<ChatModel>(messages);
            myObservableCollection = new ObservableCollection<ChatModel>(myObservableCollection.Reverse());
            return myObservableCollection;
        }
 
        public static void UpdateChat(ChatModel model)
        {
            Init();
            /*  var listchat = await db.QueryAsync<ChatListModel>("select * from ChatList where Id = ?", model.Id);
              ChatListModel chat;
              if (listchat != null)
              {
                  chat = listchat.FirstOrDefault();
                  chat.Last_message = model.Last_message;
                  chat.Updated_at = model.Updated_at;
                  await db.UpdateAsync(chat);
              }*/

        }

        public static async  Task DeleteAllMessage()
        {
            Init();
            await db.DeleteAllAsync<ChatModel>();
        }

        public static async Task<ObservableCollection<ChatModel>> GetOldMessage(int id_last_message, int Room_id)
        {
            int count_message =await CountOldMessage(id_last_message, Room_id);
            int TempCount = count_message - 5;
            string query = "Select * from ChatModel where ChatModel.chat_room_id= 1 AND ChatModel.message_id < " + id_last_message + " ORDER BY message_id ASC  limit 5 OFFSET " + TempCount;
            var listchat = await db.QueryAsync<ChatModel>(query);
            var myObservableCollection = new ObservableCollection<ChatModel>(listchat);
            return myObservableCollection;



        }
        private static async Task<int> CountOldMessage(int id_last_message,int Room_id)
        {
            string query = "Select * from ChatModel where ChatModel.chat_room_id= " + Room_id + " AND ChatModel.message_id < " + id_last_message;
            var listchat = await db.QueryAsync<ChatModel>(query);
           
         
            int count = listchat.Count();
            return count;
        }
    }
}
