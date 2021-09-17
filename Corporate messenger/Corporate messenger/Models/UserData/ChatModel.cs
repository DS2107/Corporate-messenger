using System;
using System.Collections.Generic;
using System.Text;

namespace Corporate_messenger.Models
{
    class ChatModel
    {
        public string FromUser { get; set; }
        public string ToUser { get; set; }
        public string Message { get; set; }
        public string DateSent { get; set; }
        public string IMageurl { get; set; }
        public string status { get; set; }

        List<ChatModel> messsages;

        public List<ChatModel> GetMessages(string image, string fromUser)
        {
            messsages = new List<ChatModel>() {

                new ChatModel(){ FromUser=fromUser, ToUser="Danil",Message="HI",IMageurl=image ,DateSent = DateTime.Now.ToString("HH:mm:ss"),status= "Received"},
                new ChatModel(){ FromUser="Danil", ToUser=fromUser,Message="Lorem Ipsum - это текст-рыба, часто используемый в печати и вэб-дизайне. Lorem Ipsum является стандартной рыбой для текстов на латинице с начала XVI века.",IMageurl="Person.png" ,DateSent = DateTime.Now.ToString("HH:mm:ss"),status= "Sent"},
                 new ChatModel(){ FromUser=fromUser, ToUser="Danil",Message="Многие думают, что Lorem Ipsum - взятый с потолка псевдо-латинский набор слов, но это не совсем так. Его корни уходят в один фрагмент классической латыни 45 года н.э., то есть более двух тысячелетий назад.",IMageurl=image ,DateSent = DateTime.Now.ToString("HH:mm:ss"),status= "Received"},
                 new ChatModel(){ FromUser="Danil", ToUser=fromUser,Message="Многие думают, что Lorem Ipsum - взятый с потолка псевдо-латинский набор слов, но это не совсем так. Его корни уходят в один фрагмент классической ",IMageurl="Person.png" ,DateSent = DateTime.Now.ToString("HH:mm:ss"),status= "sent"},
                 new ChatModel(){ FromUser=fromUser, ToUser="Danil",Message="Многие думают, что Lorem Ipsum - взятый с потолка псевдо-латинский набор слов, но это не совсем так. Его корни уходят в один фрагмент классической латыни 45 года н.э., то есть более двух тысячелетий назад.",IMageurl=image ,DateSent = DateTime.Now.ToString("HH:mm:ss"),status= "Received"},
                  new ChatModel(){ FromUser="Danil", ToUser=fromUser,Message="Многие думают, что Lorem Ipsum - взятый с потолка псевдо-латинский набор слов, но это не совсем так. Его корни уходят в один фрагмент классической латыни 45 года н.э., то есть более двух тысячелетий назад.",IMageurl="Person.png" ,DateSent = DateTime.Now.ToString("HH:mm:ss"),status= "sent"},
                 new ChatModel(){ FromUser=fromUser, ToUser="Danil",Message="Многие думают, что Lorem Ipsum - взятый с потолка псевдо-латинский набор слов, но это не совсем так. Его корни уходят.",IMageurl=image ,DateSent = DateTime.Now.ToString("HH:mm:ss"),status= "Received"},
                  new ChatModel(){ FromUser="Danil", ToUser=fromUser,Message="Многие думают, что Lorem Ipsum - взятый с потолка псевдо-латинский набор слов, но это не совсем так. Его корни уходят в один фрагмент классической латыни 45 года н.э., то есть более двух тысячелетий назад.",IMageurl="Person.png" ,DateSent = DateTime.Now.ToString("HH:mm:ss"),status= "sent"},
                 new ChatModel(){ FromUser=fromUser, ToUser="Danil",Message="Многие думают, что Lorem Ipsum - взятый с потолка псевдо-латинский набор слов, но это не совсем так. Его корни уходят в один фрагмент классической латыни 45 года н.э., то есть более двух тысячелетий назад.",IMageurl=image ,DateSent = DateTime.Now.ToString("HH:mm:ss"),status= "Received"}


            };
            return messsages;
        }

        public List<ChatModel> SendMessage(string text_meessage, string fromUser, List<ChatModel> myChat, string image)
        {
            messsages = myChat;
            ChatModel c = new ChatModel() { FromUser = "Danil", ToUser = fromUser, Message = text_meessage, IMageurl = "Person.png", DateSent = DateTime.Now.ToString("HH:mm:ss"), status = "sent" };
            ChatModel c2 = new ChatModel() { FromUser = fromUser, ToUser = "Danil", Message = "OK", IMageurl = image, DateSent = DateTime.Now.ToString("HH:mm:ss"), status = "Received" };
            messsages.Add(c);
            messsages.Add(c2);
            return messsages;
        }
    }
}
