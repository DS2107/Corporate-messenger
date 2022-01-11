using System;
using System.Collections.Generic;
using System.Text;

namespace Corporate_messenger.Models.Abstract
{
    public abstract class UserAbstract
    {
        /// <summary>
        /// ID пользователя
        /// </summary>
        public abstract int Id { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public abstract string Name { get; set; }
    }
}
