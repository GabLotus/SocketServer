using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Canvas
    {

        public List<User> users = new List<User>();
        public string name;
        public string strokes_collection;
        public string type;

        public Canvas(string name)
        {
            this.name = name;
        }


    }
}
