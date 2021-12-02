using System;
using System.Collections.Generic;
using System.Text;

namespace Corporate_messenger.Service.Notification
{
   public interface IForegroundService
    {
        public void StartService();
        public void StopService();

    }
}
