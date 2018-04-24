using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace Server
{
    class Database
    {
        MySqlConnection connection;
        private static Database instance;

        private Database()
        {
            string connection_string = string.Format("Server=18.219.101.114; database={0}; UID=root; password=Projet3admin", "polypaintdb");
            connection = new MySqlConnection(connection_string);
            connection.Open();
        }

        public static Database Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Database();
                }
                return instance;
            }
        }

        public string TestQuerry()
        {

            string query_string = "SELECT * FROM poly_users";
            var cmd = new MySqlCommand(query_string, connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            string str = reader.GetString(0);
            reader.Close();
            return str;

        }

        public bool AuthenticateUser(string user, string pwd)
        {
            string query_string = string.Format("SELECT * FROM poly_users WHERE user='{0}' AND password='{1}'", user, pwd);
            var cmd = new MySqlCommand(query_string, connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            bool authentified = reader.Read();
            reader.Close();
            return authentified;
        }

        public bool UserExists(string user)
        {
            string query_string = string.Format("SELECT user FROM poly_users WHERE user='{0}'", user);
            var cmd = new MySqlCommand(query_string, connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            bool exists = reader.Read();
            reader.Close();
            return exists;
        }

        public bool CanvasExists(string canvas)
        {
            string query_string = $"SELECT canvas FROM poly_canvas WHERE canvas='{canvas}'";
            var cmd = new MySqlCommand(query_string, connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            bool exists = reader.Read();
            reader.Close();
            return exists;
        }


        public void AddUser(string user, string pwd)
        {
            string query_string = string.Format("INSERT INTO poly_users (user, password) VALUES ('{0}','{1}')", user, pwd);
            var cmd = new MySqlCommand(query_string, connection);
            cmd.ExecuteNonQuery();
        }

        public Tuple<string, string> GetStrokesCollection(string canvas_names)
        {
            string query_string = string.Format("SELECT stroke_collection, type FROM poly_canvas WHERE canvas='{0}'", canvas_names);
            var cmd = new MySqlCommand(query_string, connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            bool exists = reader.Read();
            string str = reader.GetString("stroke_collection");
            string typ = reader.GetString("type");
            reader.Close();
            str = str.Replace('&', '"');
            str = str.Replace('~', '\\');
            return new Tuple<string, string>(str, typ);
        }

        public void SaveStrokesCollection(string canvas_name, string canvas_type, string stroke_collection)
        {
            stroke_collection = stroke_collection.Replace('"', '&');
            stroke_collection = stroke_collection.Replace('\\', '~');
            if (CanvasExists(canvas_name))
            {
                var query_string = $"UPDATE poly_canvas SET canvas = '{canvas_name}', type = '{canvas_type}', stroke_collection = '{stroke_collection}' WHERE canvas = '{canvas_name}'";
                var cmd = new MySqlCommand(query_string, connection);
                cmd.ExecuteNonQuery();
            }
            else
            {
                string query_string = string.Format("INSERT INTO poly_canvas (canvas, type, stroke_collection) VALUES ('{0}', '{1}', '{2}')", canvas_name, canvas_type, stroke_collection);
                var cmd = new MySqlCommand(query_string, connection);
                cmd.ExecuteNonQuery();
            }


        }

        public List<string> RequestCanvas()
        {
            var res = new List<string>();
            string query_string = "select canvas, type from poly_canvas";
            var cmd = new MySqlCommand(query_string, connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                res.Add(reader.GetString("canvas") + " - " + reader.GetString("type"));
            }

            reader.Close();
            return res;
        }
    }
}
