using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ReceiveCallbackState
    {
        public User user;
        public List<string> buffer;

        public ReceiveCallbackState(User _user)
        {
            user = _user;
            buffer = new List<string>();
        }
    }
}
