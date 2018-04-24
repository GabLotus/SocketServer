using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class CanvasManager : UserManagerObserver
    {
        public List<Canvas> canvas_list = new List<Canvas>();

        private static CanvasManager instance;

        public CanvasManager()
        {
            UserManager.Instance.client_observers.Add(this);
        }

        public static CanvasManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CanvasManager();
                }
                return instance;
            }
        }

        public List<User> FindCoUsers(User user)
        {
            var canvas = canvas_list.Where(c => c.users.Contains(user)).ToList();
            return canvas[0].users;
        }

        public override void NotifyDisconnect(User user)
        {
            foreach (Canvas canvas in canvas_list)
            {
                canvas.users.Remove(user);
            }

            canvas_list = canvas_list
                .Except(canvas_list
                .Where(c => c.users.Count < 1))
                .ToList();
            Console.WriteLine(user.username);
        }

        public string GetCollectionFrom(string canvas_name)
        {
            return canvas_list.Where(c => c.name == canvas_name).ToList()[0].strokes_collection;
        }
        
        public Canvas SelectUserCanvas(User user)
        {
            return canvas_list.Where(c => c.users.Contains(user)).ToList()[0];
        }

        public void JoinCanvas(string canvas_name, string type_req, User user)
        {
            bool joined = false;
            foreach (Canvas c in canvas_list.Where(c => c.name == canvas_name)){
                c.users.Add(user);
                joined = true;
            }
            if (!joined)
            {
                var c = new Canvas(canvas_name);
                c.type = type_req;
                c.users.Add(user);
                canvas_list.Add(c);
            }
        }

        public User GetSampleUser(string name)
        {
            return canvas_list
                .Where(c => c.name == name && c.users.Count > 0)
                .ToList()[0]
                .users[0];
        }

        public string GetCanvasType(string canvas_name)
        {
            return canvas_list.Where(c => c.name == canvas_name).ToList()[0].type;
        }

        public bool CanvasExists(string name)
        {
            return canvas_list.Where(c => c.name == name).ToList().Count() > 0;
        }


        public void DCUser(User user)
        {
            foreach (Canvas canvas in canvas_list)
            {
                canvas.users.Remove(user);
            }

            canvas_list = canvas_list.Where(c => c.users.Count > 0).ToList();

        }

    }
}
