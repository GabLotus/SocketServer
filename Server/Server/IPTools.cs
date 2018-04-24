using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    
    class IPTools
    {
        const string IP_REGEX = @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b";

        private static IPTools instance;

        private IPTools() { }

        public static IPTools Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new IPTools();
                }
                return instance;
            }


        }

        public bool Match(string ip_string)
        {
            Regex re_ip = new Regex(IP_REGEX);
            return re_ip.IsMatch(ip_string);
        }

        public IPEndPoint EndPoint(string ip_string)
        {
            return new IPEndPoint(IPAddress.Parse(ip_string), 5001);
        }
    }
}
