using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class UserAuthority
    {
        public string user;
        public List<string> ids;

        public UserAuthority(User _user)
        {
            user = _user.username;
            ids = _user.strokes;
        }
    }
}
