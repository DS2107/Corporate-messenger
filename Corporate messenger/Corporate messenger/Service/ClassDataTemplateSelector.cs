using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Corporate_messenger.Service
{
    class ClassDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FromTemplate { get; set; }
        public DataTemplate ToTemplate { get; set; }
        Models.SpecialDataModel s = new Models.SpecialDataModel();
     

           protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
         {
           return ((Models.Chat.ChatModel)item).Sender_id.Equals(s.Id) ?  ToTemplate: FromTemplate;
         }
    }
}
