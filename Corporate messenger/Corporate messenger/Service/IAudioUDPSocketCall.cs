using Corporate_messenger.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Corporate_messenger.Service
{
    public interface IAudioUDPSocketCall
    {
        /// <summary>
        /// Установить соеденение с UDP сокетом 
        /// </summary>
        public void ConnectionToServer();

        /// <summary>
        /// Получить IP удаленного соединения 
        /// </summary>
        /// <returns></returns>
        public string GetServerIp();

        /// <summary>
        /// запустить слушатель в отдельном потоке
        /// </summary>
        public void StartReceive();

     
        /// <summary>
        /// Отправка голоса в отдельном потоке
        /// </summary>
        public void SendMessage();

        /// <summary>
        /// Подготовка микрофона и динамика 
        /// </summary>
        void InitUDP();

        /// <summary>
        /// Выключить микрофон
        /// </summary>
        public void MicOff();

        /// <summary>
        /// Включить микрофон
        /// </summary>
        public void MicOn();
       

        /// <summary>
        /// Остановить все
        /// </summary>
        public void StopAudioUDPCall();

        /// <summary>
        /// Флаг поднятия трубки
        /// </summary>
        public bool FlagRaised { get; set; }
        public CallViewModel callView { get; set; }
     
    }
}
