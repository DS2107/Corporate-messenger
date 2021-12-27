using Corporate_messenger.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Corporate_messenger.Service
{
    public interface IAudioWebSocketCall
    {
        /// <summary>
        /// Инициируем стартовые настройки микрофона и динамика для вызова
        /// </summary>
        /// <param name="receiverId">Кому</param>
        /// <param name="sender_id">От кого</param>
        public void InitAudioWebSocketCall(int sender_id);
        /// <summary>
        /// Отсановка аудио вызова
        /// </summary>
        public void StopAudioWebSocketCall();

        /// <summary>
        /// Выключить микрофон
        /// </summary>
        void StopAudioRecord();


        /// <summary>
        /// Включает микрофон
        /// </summary>
        void StartAudioRecord();

        /// <summary>
        /// Старт аудио вызова
        /// </summary>
        /// <param name="ws">сокет в который будет направленн вызов</param>
        public Task StartAudioWebSocketCallAsync(WebSocketSharp.WebSocket ws);
        /// <summary>
        /// Воспроизводит аудио которое попало в сокет 
        /// </summary>
        /// <param name="audio_message">аудио</param>
        void ListenerWebSocketCall(byte[] audio_message);
        public CallViewModel callView { get; set; }

        public bool FlagRaised {get;set;}
    }
}
