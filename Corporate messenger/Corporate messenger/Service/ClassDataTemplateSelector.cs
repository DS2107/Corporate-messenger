using Corporate_messenger.DB;
using Corporate_messenger.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Corporate_messenger.Service
{
    class ClassDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FromTemplate { get; set; }
        public DataTemplate ToTemplate { get; set; }

        private UserDataModel MyUser;



        ClassDataTemplateSelector()
        {

            Task.Run(() => this.GetUser()).Wait();
        }

        protected  override DataTemplate OnSelectTemplate(object item, BindableObject container)
         {
           
           return ((Models.Chat.ChatModel)item).Sender_id.Equals(MyUser.Id) ?  ToTemplate: FromTemplate;
         }
        private async Task GetUser()
        {
            MyUser = await UserDbService.GetUser();
        }

    }
}
