using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

using System.IO;
using System.Web;

using Microsoft;
using Newtonsoft.Json;

namespace Server
{
    class User
    {
        public byte[] my_buffer = new byte[1025 * 2 * 2 * 2 * 2 * 2];
        public Socket socket;
        public bool authenticated = false;
        public string username;
        public List<string> strokes = new List<string>();

        public User(Socket _socket)
        {
            socket = _socket;
        }
    }
}
