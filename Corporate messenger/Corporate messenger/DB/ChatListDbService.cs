using SQLite;
using System.IO;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Corporate_messenger.Service;
using Corporate_messenger.Models;
using System.Collections.ObjectModel;

namespace Corporate_messenger.DB
{
    class ChatListDbService
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

        public static async Task AddChat(ChatListModel data)
        {
            Init();
            ChatListModel chat = data;

            await db.InsertAsync(chat);
        }

        public static async Task RemoveChat(int id)
        {
            Init();
            await db.DeleteAsync<ChatListModel>(id);
        }

        public static async Task<ObservableCollection<ChatListModel>> GetChats()
        {
            Init();
            var chats = await db.Table<ChatListModel>().ToListAsync();
            var myObservableCollection = new ObservableCollection<ChatListModel>(chats);
            return myObservableCollection;
        }

        public static async Task UpdateChat(ChatListModel model)
        {
            Init();
            var listchat = await db.QueryAsync<ChatListModel>("select * from ChatList where Id = ?", model.Id);
            ChatListModel chat;
            if (listchat != null)
            {
                 chat = listchat.FirstOrDefault();
                chat.Last_message = model.Last_message;
                chat.Updated_at = model.Updated_at;
                await  db.UpdateAsync(chat);
            }
        
        }
       static List<ChatListModel> list = new List<ChatListModel>();
        public static async Task<ChatListModel> GetChatId(ChatListModel model)
        {
            Init();
             list = await db.QueryAsync<ChatListModel>("select * from ChatList where Id = ?", model.Id);
            var elem = list.FirstOrDefault();
            return list.FirstOrDefault();
                        

        }

        public static async Task DeleteAllChat()
        {
            Init();
            await db.DeleteAllAsync<ChatListModel>();
        }
    }
}
