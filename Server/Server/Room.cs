using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Room
    {
        const int BUFFER_LENGTH = 15;

        public Queue<Message> message_buffer = new Queue<Message>();

        public List<User> users = new List<User>();

        public string name;

        public Room(string _name)
        {
            name = _name;
        }



        public void AddMessage(Message msg)
        {
            message_buffer.Enqueue(msg);

            if (message_buffer.Count > BUFFER_LENGTH)
            {
                message_buffer.Dequeue();
            }
        }

        public void AddUser(User user)
        {
            users.Add(user);
        }

        public bool DisconnectUser(User user)
        {
            return users.Remove(user);            
        }
        
        public static bool operator != (Room a, string b)
        {
            return a.name != b;
        }
        
        public static bool operator == (Room a, string b)
        {
            return a.name == b;
        }

        public static bool operator !=(string b, Room a)
        {
            return a.name != b;
        }

        public static bool operator ==(string b, Room a)
        {
            return a.name == b;
        }

        public void EditMessage(Message msg)
        {
            foreach (Message m in message_buffer)
            {
                if(m.id == msg.id)
                {
                    m.content = msg.content;
                }
            }
        }

        public void DeleteMessage(Message msg)
        {
            Queue<Message> temp_queue = new Queue<Message>();
            while(message_buffer.Count > 0)
            {
                var m = message_buffer.Dequeue();
                if(m.id != msg.id)
                {
                    temp_queue.Enqueue(m);
                }
            }
            while(temp_queue.Count > 0)
            {
                message_buffer.Enqueue(temp_queue.Dequeue());
            }
        }

    }
}
