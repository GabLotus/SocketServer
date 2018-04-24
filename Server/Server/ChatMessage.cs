using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ChatMessage
    {
        public string message;
        public string room;

        public ChatMessage()
        {
            room = "";
            message = "";
        }

        public ChatMessage(string _room, string _message)
        {
            room = _room;
            message = _message;
        }



    }
}
