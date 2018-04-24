using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using Microsoft;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Server
{
    class Server
    {
        private static Socket my_server =
            new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


        public static void Main()
        {
            SetupServer();
            Console.ReadLine();
        }

        public static void SetupServer()
        {
            bool connected = false;
            while (!connected)
            {
                try
                {
                    Console.WriteLine("setup");

                    string ip_string = "";
                    while (!IPTools.Instance.Match(ip_string))
                    {
                        Console.WriteLine("Enter your machine's public ip (Carte Ethernet dans ip config sur les ordinateurs de lecole) Format: XXX.XXX.XXX.XXX");
                        ip_string = Console.ReadLine();
                    }
                    my_server.Bind(IPTools.Instance.EndPoint(ip_string));
                    connected = true;
                    Console.WriteLine("Server running on: " + ip_string + ":5001");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            my_server.Listen(10);
            my_server.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        public static void AcceptCallback(IAsyncResult AR)
        {
            Socket socket = my_server.EndAccept(AR);
            try
            {
                User user = new User(socket);
                socket.BeginReceive(user.my_buffer, 0, user.my_buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveAuthentificationCallback), user);
            }
            catch (Exception err)
            {
                socket.Close();
                Console.WriteLine(err.ToString());
            }

            try
            {
                my_server.BeginAccept(new AsyncCallback(AcceptCallback), null);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
            }

        }

        public static void ReceiveAuthentificationCallback(IAsyncResult AR)
        {
            Console.WriteLine("Authentication");
            User user = (User)AR.AsyncState;
            Socket socket = user.socket;
            try
            {
                ReceiveCallbackState state = new ReceiveCallbackState(user);
                int received = user.socket.EndReceive(AR);
                StreamParser(received, state);
                string msg_string = state.buffer.First();
                state.buffer.RemoveAt(0);
                Message message = MessageParser(msg_string);
                Message res = AuthenticationHandler.Instance.Handle(user, message);
                SendMessage(user, res);
                if (user.authenticated)
                {
                    user.socket.BeginReceive(user.my_buffer, 0, user.my_buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
                }
            }
            catch (Exception my_exception)
            {
                UserManager.Instance.DisconnectUser(user);
                Console.WriteLine(my_exception.ToString());
            }
        }

        public static void ReceiveCallback(IAsyncResult AR)
        {
            Console.WriteLine("Receive Callback");
            ReceiveCallbackState state = (ReceiveCallbackState)AR.AsyncState;
            User user = state.user;
            try
            {
                int received = user.socket.EndReceive(AR);
                Console.WriteLine(received);
                StreamParser(received, state);
                if (received > 0)
                {
                    while (state.buffer.Count > 0 && MessageComplete(state.buffer.First()))
                    {

                        string msg_string = state.buffer.First();
                        Message message = MessageParser(msg_string);
                        state.buffer.RemoveAt(0);

                        var response_object = MessageHandler.Instance.Handle(message, user);

                        foreach (User usr in response_object.Item2.Where(u => u.authenticated == true))
                        {
                            foreach (Message msg in response_object.Item1)
                            {
                                SendMessage(usr, msg);
                            }
                        }
                    }
                    user.socket.BeginReceive(user.my_buffer, 0, user.my_buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    UserManager.Instance.DisconnectUser(user);
                    //user.socket.BeginReceive(user.my_buffer, 0, user.my_buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
                    Console.WriteLine(user.username);
                }
            }
            catch (Exception my_excepion)//excepion?
            {
                UserManager.Instance.DisconnectUser(user);
                Console.WriteLine(my_excepion.Message);
            }

            Console.WriteLine("Receivecallback finished");
        }

        public static void SendCallback(IAsyncResult AR)
        {
            User user = (User)AR.AsyncState;
            try
            {
                user.socket.EndSend(AR);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
            }
            try
            {
                if (!user.authenticated)
                {
                    user.socket.Close();
                    UserManager.Instance.my_clients.Remove(user);
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
            }

        }

        public static void StreamParser(int n_bytes, ReceiveCallbackState state)
        {
            byte[] data_buffer = new byte[n_bytes];
            Array.Copy(state.user.my_buffer, data_buffer, n_bytes);
            string text = Encoding.UTF8.GetString(data_buffer);
            Regex regex = new Regex("!END");
            List<string> split_buffer = Regex.Split(text, @"(?<=!END)").ToList();
            foreach (string s in split_buffer)
            {

                if (state.buffer.Count == 0 || MessageComplete(state.buffer.First()))
                {
                    state.buffer.Add(s);
                }
                else
                {
                    state.buffer[0] = state.buffer[0] + s;
                }
            }

        }

        public static bool MessageComplete(string s)
        {
            if (s.Length > 4)
            {
                return s.Substring(s.Length - 4) == "!END";
            }
            else
            {
                return false;
            }
        }


        public static Message MessageParser(string s)
        {
            s = s.Substring(0, s.Length - 4);
            return JsonConvert.DeserializeObject<Message>(s);
        }

        public static byte[] MessageSerializer(Message message)
        {
            string data_string = JsonConvert.SerializeObject(message);
            data_string += "!END";
            return Encoding.UTF8.GetBytes(data_string);
        }

        public static void SendMessage(User user, Message message)
        {
            byte[] data_send = MessageSerializer(message);
            user.socket.BeginSend(data_send, 0, data_send.Length, SocketFlags.None, new AsyncCallback(SendCallback), user);
        }
    }
}
