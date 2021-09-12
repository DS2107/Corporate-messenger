﻿using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Corporate_messenger.Service
{
    class ClassDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FromTemplate { get; set; }
        public DataTemplate ToTemplate { get; set; }
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return ((Models.Chat)item).status.ToUpper().Equals("SENT") ? FromTemplate : ToTemplate;
        }
    }
}