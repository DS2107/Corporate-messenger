using Corporate_messenger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Corporate_messenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
      //  public string mat { get { return mat; } set  { mat = "Red"; } }
       
        string image1 = "";
        string title1 = "";
        List<Chat> dsfdsf = new List<Chat>();

        public ChatPage(string title, string image)
        {
            InitializeComponent();
         
         

          //  Shell.SetBackgroundColor(this, value);
             // mat = "Red";
             image1 = image;
            title1 = title;
            dsfdsf = new Chat().GetMessages(image, title);         
            MyListView.ItemsSource = new Chat().GetMessages(image, title);
            Title = title;
        }

        private void SetTitleColor(ChatPage chatPage, Color red)
        {
            throw new NotImplementedException();
        }

        Chat c = new Chat();
        private void send_message_Clicked(object sender, EventArgs e)
        {
            MyListView.ItemsSource = null;

            MyListView.ItemsSource = c.SendMessage(MessageEditor.Text, title1, dsfdsf, image1);
            MessageEditor.Text = "";
            object d = 0;
            foreach (var s in MyListView.ItemsSource)
            {
                d = s;
            }
            MyListView.ScrollTo(d, ScrollToPosition.End, true);
        }
    }
}