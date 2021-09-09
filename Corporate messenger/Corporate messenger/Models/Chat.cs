using System;
using System.Collections.Generic;
using System.Text;

namespace Corporate_messenger.Models
{
    class Chat
    {
        public string FromUser { get; set; }
        public string ToUser { get; set; }
        public string Message { get; set; }
        public string DateSent { get; set; }
        public string IMageurl { get; set; }
        public string status { get; set; }

        List<Chat> messsages;

        public List<Chat> GetMessages(string image, string fromUser)
        {
            messsages = new List<Chat>() {

                new Chat(){ FromUser=fromUser, ToUser="Danil",Message="HI",IMageurl=image ,DateSent = DateTime.Now.ToString("HH:mm:ss"),status= "Received"},
                new Chat(){ FromUser="Danil", ToUser=fromUser,Message="Lorem Ipsum - это текст-рыба, часто используемый в печати и вэб-дизайне. Lorem Ipsum является стандартной рыбой для текстов на латинице с начала XVI века.",IMageurl="Person.png" ,DateSent = DateTime.Now.ToString("HH:mm:ss"),status= "Sent"},
                 new Chat(){ FromUser=fromUser, ToUser="Danil",Message="Многие думают, что Lorem Ipsum - взятый с потолка псевдо-латинский набор слов, но это не совсем так. Его корни уходят в один фрагмент классической латыни 45 года н.э., то есть более двух тысячелетий назад.",IMageurl=image ,DateSent = DateTime.Now.ToString("HH:mm:ss"),status= "Received"},
                 new Chat(){ FromUser="Danil", ToUser=fromUser,Message="Многие думают, что Lorem Ipsum - взятый с потолка псевдо-латинский набор слов, но это не совсем так. Его корни уходят в один фрагмент классической ",IMageurl="Person.png" ,DateSent = DateTime.Now.ToString("HH:mm:ss"),status= "sent"},
                 new Chat(){ FromUser=fromUser, ToUser="Danil",Message="Многие думают, что Lorem Ipsum - взятый с потолка псевдо-латинский набор слов, но это не совсем так. Его корни уходят в один фрагмент классической латыни 45 года н.э., то есть более двух тысячелетий назад.",IMageurl=image ,DateSent = DateTime.Now.ToString("HH:mm:ss"),status= "Received"},
                  new Chat(){ FromUser="Danil", ToUser=fromUser,Message="Многие думают, что Lorem Ipsum - взятый с потолка псевдо-латинский набор слов, но это не совсем так. Его корни уходят в один фрагмент классической латыни 45 года н.э., то есть более двух тысячелетий назад.",IMageurl="Person.png" ,DateSent = DateTime.Now.ToString("HH:mm:ss"),status= "sent"},
                 new Chat(){ FromUser=fromUser, ToUser="Danil",Message="Многие думают, что Lorem Ipsum - взятый с потолка псевдо-латинский набор слов, но это не совсем так. Его корни уходят.",IMageurl=image ,DateSent = DateTime.Now.ToString("HH:mm:ss"),status= "Received"},
                  new Chat(){ FromUser="Danil", ToUser=fromUser,Message="Многие думают, что Lorem Ipsum - взятый с потолка псевдо-латинский набор слов, но это не совсем так. Его корни уходят в один фрагмент классической латыни 45 года н.э., то есть более двух тысячелетий назад.",IMageurl="Person.png" ,DateSent = DateTime.Now.ToString("HH:mm:ss"),status= "sent"},
                 new Chat(){ FromUser=fromUser, ToUser="Danil",Message="Многие думают, что Lorem Ipsum - взятый с потолка псевдо-латинский набор слов, но это не совсем так. Его корни уходят в один фрагмент классической латыни 45 года н.э., то есть более двух тысячелетий назад.",IMageurl=image ,DateSent = DateTime.Now.ToString("HH:mm:ss"),status= "Received"}


            };
            return messsages;
        }

        public List<Chat> SendMessage(string text_meessage, string fromUser, List<Chat> myChat, string image)
        {
            messsages = myChat;
            Chat c = new Chat() { FromUser = "Danil", ToUser = fromUser, Message = text_meessage, IMageurl = "Person.png", DateSent = DateTime.Now.ToString("HH:mm:ss"), status = "sent" };
            Chat c2 = new Chat() { FromUser = fromUser, ToUser = "Danil", Message = "OK", IMageurl = image, DateSent = DateTime.Now.ToString("HH:mm:ss"), status = "Received" };
            messsages.Add(c);
            messsages.Add(c2);
            return messsages;
        }
    }
}
