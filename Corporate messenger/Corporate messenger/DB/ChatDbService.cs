using Corporate_messenger.Models.Chat;
using Corporate_messenger.Service;
using SQLite;
using System.Linq;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

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
        public static async Task<ChatModel> GetLastMessages()
        {
            Init();
            var messages = await db.Table<ChatModel>().ToListAsync();
            var myObservableCollection = new ObservableCollection<ChatModel>(messages);
            return myObservableCollection.Last();
        }
        public static async Task<ObservableCollection<ChatModel>> GetMessages(int id)
        {
            Init();
            var listchat = await db.QueryAsync<ChatModel>("select * from ChatModel where Chat_room_id = ?", id);
          //  var messages = await db.Table<ChatModel>().ToListAsync();
            var myObservableCollection = new ObservableCollection<ChatModel>(listchat);
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
    }
}
