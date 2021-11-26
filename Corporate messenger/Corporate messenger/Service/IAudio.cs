using System;
using System.Collections.Generic;
using System.Text;

namespace Corporate_messenger.Service
{
   
    public interface IAudio
    {
        /// <summary>
        /// Воспроизводит файл
        /// </summary>
        /// <param name="fileName">файл</param>
        void PlayAudioFile(string fileName);

        /// <summary>
        /// Отправить Аудио сообщение
        /// </summary>
        void SendMessageAudio();

        /// <summary>
        /// Прекращает запись сообщения и отправляет его 
        /// </summary>
        void StopSendMessageAudio();

        /// <summary>
        /// Прекращает воспроизведение файла
        /// </summary>
        void StopAudioFile();

        /// <summary>
        /// Вернуть ввсе время аудио сообщения
        /// </summary>
        /// <returns></returns>
        public double GetFullTimeAudio();

        /// <summary>
        /// Вернуть текущую позицию
        /// </summary>
        /// <returns></returns>
        public double GetPositionAudio();

        /// <summary>
        /// Продолжить аудио
        /// </summary>
        public void ResumeAudioFile();
    }
}
