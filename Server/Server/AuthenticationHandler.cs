using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class AuthenticationHandler
    {
        private static AuthenticationHandler instance;

        private AuthenticationHandler() { }

        public static AuthenticationHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AuthenticationHandler();
                }
                return instance;
            }


        }
        
        public Message Handle(User user, Message req)
        {
            Message res = new Message();
            string error_message = "error: ";
            user.username = req.username;
            res.type = "authentication_response";

            bool user_length = req.username.Length > 0;
            bool pwd_length = req.content.Length > 0;

            if (req.type == "authentication")
            {
                //TODO: Querry DB
                bool authentication_confirmed = Database.Instance.AuthenticateUser(req.username, req.content); // <--- TODO: IMPLEMENT AUTHENTIFICATION
                bool unique_user = UniqueUser(user);
                Console.WriteLine("User trying to connect: " + user.username + " " + req.content);
                if (authentication_confirmed && UniqueUser(user) && user_length && pwd_length)
                {
                    res.content = "success";
                    Authentify(user);
                }
                else
                {
                    error_message += ((authentication_confirmed ? "" : "wrong user or password, ")
                        + (unique_user ? "" : "user already connected, ")
                        + (user_length ? "" : "no username, ")
                        + (pwd_length ? "" : "no password"));
                    res.content = error_message;
                }
            }
            else if(req.type == "create_user")
            {
                bool user_registered = Database.Instance.UserExists(req.username);
                if (!user_registered && user_length && pwd_length)
                {
                    Database.Instance.AddUser(req.username, req.content);
                    res.content = "success";
                    Authentify(user);
                }
                else
                {
                    error_message += ((!user_registered ? "" : "user already registered, ")
                        + (pwd_length ? "" : "no password, ")
                        + (user_length ? "" : "no username, "));
                    res.content = error_message;
                }

            }
            else
            {
                res.content = "error: no authentication request";
            }

            return res;
        }


        private void Authentify(User user)
        {
            user.authenticated = true;
            UserManager.Instance.my_clients.Add(user);
        }

        private bool UniqueUser(User user)
        {
            foreach (User usr in UserManager.Instance.my_clients)
            {
                if (usr.username == user.username)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
