using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    abstract class UserManagerObserver
    {
        public abstract void NotifyDisconnect(User user);        
    }
}
