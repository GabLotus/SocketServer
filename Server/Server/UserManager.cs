using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class UserManager
    {

        public List<User> my_clients = new List<User>();
        public List<UserManagerObserver> client_observers = new List<UserManagerObserver>();

        private static UserManager instance;

        private UserManager() { }

        public static UserManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UserManager();
                }

                return instance;
            }
        }

        public void DisconnectUser(User user)
        {     
            my_clients.Remove(user);

            foreach (UserManagerObserver obs in client_observers)
            {
                obs.NotifyDisconnect(user);
            }

            user.socket.Close();
        }

    }
}
