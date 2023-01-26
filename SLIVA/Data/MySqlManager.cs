using System;
using MySql.Data.MySqlClient;
using SLIVA.Models;
using System.Collections.Generic;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using System.Text;
using System.Linq;
namespace SLIVA.Data
{
    public class MySqlManager
    {
        MySqlConnection connection = new MySqlConnection("Server=127.0.0.1;UserId=root;Password=55665566;Database=SLIVA_db;");
        #region User-related
        #region GetUserMethods
        public User GetUser(UserAuthenticationData auth_data, Client current_client)
        {
            connection.Open();
            var command = new MySqlCommand($"SELECT * FROM Users WHERE user_login = '{auth_data.Login}';", connection);
            var reader = command.ExecuteReader();
            reader.Read();
            if (!reader.HasRows)
            {
                connection.Close();
                Console.WriteLine("User does not exist!");
                return null;
            }

            string user_password = reader.GetValue(0).ToString();
            if (auth_data.Password == user_password)
            {
                int user_id = reader.GetInt32(1);
                string user_login = reader.GetValue(2).ToString();

                reader.Close();
                connection.Close();

                return new User(user_login, user_password, user_id, current_client);
            }

            reader.Close();
            connection.Close();
            return null;

        }
        public string GetUserLoginById(int id)
        {
            connection.Open();
            var command = new MySqlCommand($"SELECT user_login FROM Users WHERE user_id = '{id}';", connection);
            var reader = command.ExecuteReader();
            reader.Read();
            if (!reader.HasRows)
            {
                reader.Close();
                connection.Close();
                Console.WriteLine("There is no user with id " + id);
                return null;
            }
            string result = reader.GetString(0);
            reader.Close();
            connection.Close();
            return result;

        }
        public User GetUserOfClient(Client client)
        {
            connection.Open();
            var command = new MySqlCommand($"SELECT user_id FROM Sessions WHERE client_id = '{client.Id}';", connection);
            var reader = command.ExecuteReader();
            reader.Read();
            if (!reader.HasRows)
            {
                reader.Close();
                connection.Close();
                Console.WriteLine("There are no sessions for this user!");
                return null;
            }

            int user_id = reader.GetInt32(0);
            Console.WriteLine(user_id);
            reader.Close();

            command = new MySqlCommand($"SELECT * FROM Users WHERE user_id = '{user_id}';", connection);
            reader = command.ExecuteReader();
            reader.Read();

                string user_password = reader.GetValue(0).ToString();

                string user_login = reader.GetValue(2).ToString();

                reader.Close();
                connection.Close();

                return new User(user_login, user_password, user_id, client);


            reader.Close();
            connection.Close();
            return null;

        }
        public User GetUser(string login, Client current_client)
        {
            connection.Open();
            var command = new MySqlCommand($"SELECT * FROM Users WHERE user_login = '{login}';", connection);
            var reader = command.ExecuteReader();
            reader.Read();
            if (!reader.HasRows)
            {
                connection.Close();
                Console.WriteLine("User does not exist!");
                return null;
            }

                string user_password = reader.GetValue(0).ToString();
                int user_id = reader.GetInt32(1);
                string user_login = reader.GetValue(2).ToString();

                reader.Close();
                connection.Close();

                return new User(user_login, user_password, user_id,current_client);

        }
        #endregion GetUserMethods
        public UserRegistrationStatus RegistrateUser(UserRegistrationRequest registrationData, Client current_client)
        {
            connection.Open();
            var command = new MySqlCommand($"SELECT * FROM Users WHERE user_login = '{registrationData.Login}';", connection);
            var reader = command.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                reader.Close();
                connection.Close();
                Console.WriteLine("User already exists!");
                return UserRegistrationStatus.LoginTaken;
            }
            reader.Close();

            command = new MySqlCommand($"INSERT INTO Users(user_login,user_password)" +
            	$"VALUES('{registrationData.Login}','{registrationData.Password}');",connection);

            try
            {
                command.ExecuteNonQuery();
                connection.Close();
                return UserRegistrationStatus.Done;
            }
            catch(Exception ex)
            {
                if (ex.Message.Contains("user_password"))
                {
                    Console.WriteLine("Incorrect password : " + registrationData.Password);
                    return UserRegistrationStatus.UnappropriateSignPassword;
                }
            }

            reader.Close();
            connection.Close();
            return UserRegistrationStatus.UnexpectedFailure;
        }

        public bool UserExists(string login)
        {
            connection.Open();
            var command = new MySqlCommand($"SELECT * FROM Users WHERE user_login = '{login}';",connection);

            var reader = command.ExecuteReader();
            reader.Read();

            if (reader.HasRows)
            {
                connection.Close();
                reader.Close();
                return true;
            }
            reader.Close();
            connection.Close();

            return false;
        }
        public void ConsoleWriteUsers()
        {
            using (connection)
            {
                connection.Open();

                int row_count = Convert.ToInt32(new MySqlCommand("SELECT COUNT(user_id) FROM Users", connection).ExecuteScalar());

                Console.WriteLine("Users amount : " + row_count);

                var command = new MySqlCommand("SELECT * FROM Users", connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    for (int i = reader.FieldCount - 1; i > 0; i--)
                        Console.WriteLine(reader.GetName(i) + " : " + reader[i]);
                    Console.WriteLine();
                }
                reader.Close();
                connection.Close();
            }
        }

        public bool SessionExists(Client client)
        {
            bool result = false;
            connection.Open();
            var command = new MySqlCommand($"SELECT * FROM Sessions WHERE client_id = {client.Id};", connection);

            var reader = command.ExecuteReader();
            reader.Read();

            if (reader.HasRows)
            {
                connection.Close();
                reader.Close();
                result = true;
            }
            reader.Close();
            connection.Close();
            Console.WriteLine("session exists : " + result);
            return result;

        }

        public void CreateSession(User user, string sid)
        {

            if (SessionExists(user.Current_Client))
                EliminateSessionForClient(user.Current_Client.Id);
            else
            {
                using (connection)
                {

                    connection.Open();
                    var command = new MySqlCommand($"INSERT INTO Sessions(session_id,user_id,client_id) VALUES('{sid}',{user.Id},{user.Current_Client.Id});", connection);
                    command.ExecuteNonQuery();
                    connection.Close();

                }
                Console.WriteLine($"Session with {sid} has been created");
            }            
        }

        public void EliminateSessionForClient(int client_id)
        {
            
            using (connection)
            {
                connection.Open();
                var command = new MySqlCommand($"DELETE FROM Sessions WHERE client_id = {client_id};", connection);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        public string GenerateSID(User user)
        {
            string sid = "SID:" + user.Login + Convert.ToString((user.Id+1493)*DateTime.Now.Second + user.Login.Length);
            
            Console.WriteLine(sid);



            return sid;
        }
        #endregion

        #region Client-related
        public void ConsoleWriteClients()
        {
            using (connection)
            {
                connection.Open();

                int row_count = Convert.ToInt32(new MySqlCommand("SELECT COUNT(client_ip) FROM Clients", connection).ExecuteScalar());

                Console.WriteLine("Clients amount : " + row_count);

                var command = new MySqlCommand("SELECT * FROM Clients", connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    for (int i = reader.FieldCount - 1; i > 0; i--)
                    {
                        Console.WriteLine(reader.GetName(i) + " : " + reader.GetValue(1) );

                    }
                    Console.WriteLine();
                }
                reader.Close();
                connection.Close();
            }
        }

        public void InsertClient(string ip)
        {

            try
            {
                using (connection)
                {
                    connection.Open();
                    var command = new MySqlCommand($"INSERT INTO Clients(client_ip) VALUES(INET_ATON('{ip}'))", connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }

            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex);
            }
        }

        public Client GetClientByIp(string ip)
        {
            Client result = null;
            try
            {
                using (connection)
                {
                    connection.Open();

                    var command = new MySqlCommand($"SELECT * FROM Clients WHERE client_ip = INET_ATON('{ip}');", connection);
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        result = new Client(reader.GetString(1), reader.GetInt16(0));



                        connection.Close();
                        reader.Close();
                    }
                    reader.Close();
                    connection.Close();
                    return result;
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex);
                connection.Close();
                return result;
            }
        }

        public bool ClientExists(string ip)
        {
            bool result = false;
            try
            {
                using (connection)
                {
                    connection.Open();
                    Console.WriteLine(ip);
                    var command = new MySqlCommand($"SELECT * FROM Clients WHERE client_ip = INET_ATON('{ip}');", connection);
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Close();
                        connection.Close();
                        result = true;
                    }
                    reader.Close();
                    connection.Close();
                    Console.WriteLine("CLIENT EXISTS : " + result);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                connection.Close();
                return false;
            }
        }
        #endregion

        #region MessageRelated

        public MessageSendingStatus InsertMessage(MessagePostRequest messagePostData,User author)
        {
            bool content_is_appropriate = true;
            if (!content_is_appropriate)
            {
                return MessageSendingStatus.InappropriateContent;
            }
            if(author == null)
            {
                return MessageSendingStatus.SessionProblem;
            }

            try
            {
                DateTime time = DateTime.Now;
                string format = "yyyy-MM-dd HH:mm:ss";
                Console.WriteLine(DateTime.Now.ToString());
                connection.Open();
                var command = new MySqlCommand($"INSERT INTO Messages(message_author_id,message_client_id,message_sent_at,message_content) " +
                	$"VALUES({author.Id},{author.Current_Client.Id},'{time.ToString(format)}','{messagePostData.Content}') ", connection);
                command.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return MessageSendingStatus.Error;
            }

            return MessageSendingStatus.Sent;

        }

        public Message GetMessage(int message_id)
        {
            connection.Open();
            var command = new MySqlCommand($"SELECT * FROM Messages WHERE message_id = '{message_id}';", connection);
            var reader = command.ExecuteReader();
            reader.Read();
            if (!reader.HasRows)
            {
                connection.Close();
                Console.WriteLine("Message does not exist!");
                return null;
            }

                int author_id = reader.GetInt32(1);
                int client_id = reader.GetInt32(2);
                DateTime sent_at = reader.GetDateTime(3);
                string content = reader.GetString(4);

                reader.Close();
                connection.Close();

                return new Message(content,author_id,client_id,message_id);

        }
        public List<Message> GetLatestMessages(int amount)
        {

            int message_id;
            int author_id;
            int client_id;
            DateTime sent_at;
            string content;


            connection.Open();
            var command = new MySqlCommand($"SELECT * FROM Messages ORDER BY message_sent_at DESC LIMIT {amount};", connection);
            var reader = command.ExecuteReader();
            List<Message> result_list = new List<Message>();
            while (reader.Read())
            {
                message_id = reader.GetInt32(0);
                author_id = reader.GetInt32(1);
                client_id = reader.GetInt32(2);
                sent_at = reader.GetDateTime(3);
                content = reader.GetString(4);

                result_list.Add(new Message(content,sent_at, author_id, client_id, message_id));
            }

                reader.Close();
                connection.Close();
                return result_list;
        }
        #endregion
    }
}


