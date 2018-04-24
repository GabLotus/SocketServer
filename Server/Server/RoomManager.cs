using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class RoomManager : UserManagerObserver
    {

        public List<Room> rooms = new List<Room>();

        private static RoomManager instance;

        private RoomManager()
        {
            UserManager.Instance.client_observers.Add(this);
        }

        public static RoomManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RoomManager();
                }
                return instance;
            }
        }

        public override void NotifyDisconnect(User user)
        {
            foreach (Room room in rooms)
            {
                Console.WriteLine("user:" + user.username + ", disconnected");
                room.DisconnectUser(user);
            }

            rooms = rooms.Where(r => r.users.Count > 0).ToList();
        }

        public Room JoinRoom(string room_name, User user)
        {
            Room joined_room = new Room("");
            foreach (Room room in rooms.Where(r => r == room_name))
            {
                room.AddUser(user);
                joined_room = room;             
            }
            if (joined_room == "")
            {
                joined_room = new Room(room_name);
                joined_room.AddUser(user);
                rooms.Add(joined_room);
                Console.WriteLine("User: " + user.username + ", joined room: " + joined_room.name);
            }
            return joined_room;
        }
    }
}
