using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Message
    {
        public string type;
        public string action;
        public DateTime date_time;
        public string id;
        public string content;
        public string username;

        public Message()
        {
            this.username = "";
            this.type = "";
            this.action = "";
            this.date_time = DateTime.ParseExact(DateTime.Now.ToString("HH:mm:ss"), "HH:mm:ss", System.Globalization.CultureInfo.InstalledUICulture);
            this.id = "";
            this.content = "";
        }

       

    }
}
