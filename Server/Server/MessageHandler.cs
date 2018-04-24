using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Server
{
    class MessageHandler
    {
        private static MessageHandler instance;

        private MessageHandler() { }

        public static MessageHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MessageHandler();
                }
                return instance;
            }
        }

        public Tuple<List<Message>, List<User>> Handle(Message req, User user)
        {
            var res = new Tuple<List<Message>, List<User>>(new List<Message>(), new List<User>());
            switch (req.type)
            {
                case "message":
                    res = HandleChatMessage(req, user);
                    Console.WriteLine("Handled a message");
                    Console.WriteLine("Sending " + res.Item1.Count + " messages to " + res.Item2.Count + " users");
                    break;
                case "join_canvas":
                    res = JoinCanvas(req, user);
                    Console.WriteLine("Handled a join_canvas");
                    break;
                case "join_room":
                    res = JoinRoom(req, user);
                    Console.WriteLine("Handled a join_room");
                    Console.WriteLine("Sending " + res.Item1.Count + " messages to " + res.Item2.Count + " users");
                    break;
                case "select":
                    res = HandleSelectMessage(req, user);
                    Console.WriteLine("Handled a select");
                    break;
                case "quit_room":
                    res = QuitRoom(req, user);
                    Console.WriteLine("Handled a quit_room");
                    break;
                case "update_canvas":
                    res = UpdateCanvas(req, user);
                    Console.WriteLine("Handled a update_canvas");
                    break;
                case "save_canvas":
                    res = SaveCanvas(req, user);
                    Console.WriteLine("Handled a save_canvas");
                    break;
                case "request_canvas":
                    res = RequestCanvas(req, user);
                    Console.WriteLine("Handled a request_canvas");
                    break;
                case "edit_message":
                    res = EditMessage(req, user);
                    break;
                case "delete_message":
                    res = DeleteMessage(req, user);
                    break;
                case "request_collection":
                    res = RequestCollection(req, user);
                    break;
                case "request_update":
                    res = RequestUpdate(req, user);
                    break;
                default:
                    Console.WriteLine("Default handle");
                    res.Item1.Add(req);
                    foreach (User usr in CanvasManager.Instance.FindCoUsers(user))
                    {
                        res.Item2.Add(usr);
                    }
                    break;
            }

            return res;
        }

        public Tuple<List<Message>, List<User>> EditMessage(Message req, User user)
        {
            var chat_content = JsonConvert.DeserializeObject<ChatMessage>(req.content);



            var user_list = new List<User>();
            var res_list = new List<Message>();

            foreach (Room room in RoomManager.Instance.rooms.Where(r => r == chat_content.room))
            {
                user_list = room.users;
                room.EditMessage(req);
            }

            res_list.Add(req);

            return new Tuple<List<Message>, List<User>>(res_list, user_list);
        }

        public Tuple<List<Message>, List<User>> DeleteMessage(Message req, User user)
        {
            var chat_content = JsonConvert.DeserializeObject<ChatMessage>(req.content);



            var user_list = new List<User>();
            var res_list = new List<Message>();

            foreach (Room room in RoomManager.Instance.rooms.Where(r => r == chat_content.room))
            {
                user_list = room.users;
                room.DeleteMessage(req);
            }

            res_list.Add(req);

            return new Tuple<List<Message>, List<User>>(res_list, user_list);
        }

        public Tuple<List<Message>, List<User>> UpdateCanvas(Message req, User user)
        {
            var user_list = new List<User>();
            user_list = CanvasManager.Instance.FindCoUsers(user);

            var res_list = new List<Message>();
            req.type = "image";
            req.action = "update";
            res_list.Add(req);

            string updated_collection = req.content;
            CanvasManager.Instance.SelectUserCanvas(user).strokes_collection = updated_collection;

            return new Tuple<List<Message>, List<User>>(res_list, user_list);
        }

        public Tuple<List<Message>, List<User>> RequestCanvas(Message req, User user)
        {
            var user_list = new List<User>();
            user_list.Add(user);

            var res_list = new List<Message>();
            var canvas_list = Database.Instance.RequestCanvas();
            req.content = JsonConvert.SerializeObject(canvas_list);
            res_list.Add(req);

            return new Tuple<List<Message>, List<User>>(res_list, user_list);
        }

        public Tuple<List<Message>, List<User>> SaveCanvas(Message req, User user)
        {
            var user_list = new List<User>();
            var res_list = new List<Message>();

            var save_params = JsonConvert.DeserializeObject<List<string>>(req.content);

            Database.Instance.SaveStrokesCollection(save_params[0], save_params[1], save_params[2]);


            return new Tuple<List<Message>, List<User>>(res_list, user_list);
        }

        public Tuple<List<Message>, List<User>> JoinCanvas(Message req, User user)
        {
            var user_list = new List<User>();
            var canvas_to_join = req.content;
            var res_list = new List<Message>();
            CanvasManager.Instance.DCUser(user);
            if (CanvasManager.Instance.CanvasExists(canvas_to_join)
                && req.action != CanvasManager.Instance.GetCanvasType(canvas_to_join))
            {
                UserManager.Instance.DisconnectUser(user);
            }
            else
            {
                var canvas_type = req.action;
                if (CanvasManager.Instance.CanvasExists(canvas_to_join))
                {
                    user_list.Add(CanvasManager.Instance.GetSampleUser(req.content));
                    req = new Message();
                    req.type = "update_canvas";
                    req.content = "";
                }
                else
                {
                    req = new Message();
                    req.type = "image";
                    req.action = "update";
                    req.id = "none";
                    user_list.Add(user);

                }
                CanvasManager.Instance.JoinCanvas(canvas_to_join, canvas_type, user);
                res_list.Add(req);
            }
            return new Tuple<List<Message>, List<User>>(res_list, user_list);
        }

        public Tuple<List<Message>, List<User>> QuitRoom(Message req, User user)
        {
            string room_name = req.content;
            List<User> user_list = new List<User>();
            foreach (Room room in RoomManager.Instance.rooms.Where(r => r.name == room_name))
            {
                room.users.Remove(user);
                user_list = room.users;
            }
            Message msg = new Message();
            string res_message = "Announcement: " + user.username + " left the room";
            string chat_message = JsonConvert.SerializeObject(new ChatMessage(room_name, res_message));
            msg.type = "message";
            msg.content = chat_message;

            List<Message> message_list = new List<Message>();
            message_list.Add(msg);

            return new Tuple<List<Message>, List<User>>(message_list, user_list);

        }

        public Tuple<List<Message>, List<User>> JoinRoom(Message req, User user)
        {
            string room_name = req.content;

            var user_list = new List<User>();
            user_list.Add(user);

            var room = RoomManager.Instance.JoinRoom(room_name, user);
            req.content = req.username + " joined room:" + req.content;

            var res_list = new List<Message>();
            res_list.Add(req);

            foreach (Message msg in room.message_buffer)
            {
                res_list.Add(msg);
            }

            return new Tuple<List<Message>, List<User>>(res_list, user_list);
        }

        public Tuple<List<Message>, List<User>> HandleChatMessage(Message req, User user)
        {
            var req_content = JsonConvert.DeserializeObject<ChatMessage>(req.content);
            Console.WriteLine("Room: " + req_content.room + ", Message: " + req_content.message);
            List<User> res_users = new List<User>();

            foreach (Room room in RoomManager.Instance.rooms.Where(r => r == req_content.room))
            {
                res_users = room.users;
                room.AddMessage(req);
            }
            var res_list = new List<Message>();
            res_list.Add(req);
            return new Tuple<List<Message>, List<User>>(res_list, res_users);
        }

        public Tuple<List<Message>, List<User>> HandleSelectMessage(Message req, User user)
        {
            // req
            List<string> new_authority = JsonConvert.DeserializeObject<List<string>>(req.content);
            user.strokes = new_authority;

            // res
            var users = CanvasManager.Instance.FindCoUsers(user);
            //var users = UserManager.Instance.my_clients;
            var res = new Tuple<List<Message>, List<User>>(new List<Message>(), users);

            var authority_list = new List<UserAuthority>();
            foreach (User usr in users)
            {
                authority_list.Add(new UserAuthority(usr));
            }

            string authority_string = JsonConvert.SerializeObject(authority_list);

            var msg = new Message();
            msg.type = "select";
            msg.content = authority_string;

            res.Item1.Add(msg);

            return res;

        }


        public Tuple<List<Message>, List<User>> RequestCollection(Message req, User user)
        {
            var user_list = new List<User>();
            user_list = (CanvasManager.Instance.FindCoUsers(user));

            var database_res = Database.Instance.GetStrokesCollection(req.content);
            req.content = database_res.Item1;
            req.action = database_res.Item2;
            var res_list = new List<Message>();
            res_list.Add(req);
            return new Tuple<List<Message>, List<User>>(res_list, user_list);
        }


        public Tuple<List<Message>, List<User>> RequestUpdate(Message req, User user)
        {
            var user_list = new List<User>();
            var res_list = new List<Message>();
            return new Tuple<List<Message>, List<User>>(res_list, user_list);
        }

        /*
        public Tuple<List<Message>, List<User>> MethodName(Message req, User user)
        {
            var user_list = new List<User>();
            var res_list = new List<Message>();
            return new Tuple<List<Message>, List<User>>(res_list, user_list);
        }
        */
    }
}
