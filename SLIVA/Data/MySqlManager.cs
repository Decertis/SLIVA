using System;
using MySql.Data.MySqlClient;
using SLIVA.Models;
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

            string user_password = reader.GetValue(1).ToString();
            if (auth_data.Password == user_password)
            {
                string user_name = reader.GetValue(0).ToString();
                string user_email = reader.GetValue(2).ToString();
                int user_id = reader.GetInt32(3);
                string user_login = reader.GetValue(4).ToString();

                reader.Close();
                connection.Close();

                return new User(user_name, user_login, user_password, user_email, user_id, current_client);
            }

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

                string user_password = reader.GetValue(1).ToString();
                string user_name = reader.GetValue(0).ToString();
                string user_email = reader.GetValue(2).ToString();
                int user_id = reader.GetInt32(3);
                string user_login = reader.GetValue(4).ToString();

                reader.Close();
                connection.Close();

                return new User(user_name, user_login, user_password, user_email, user_id,current_client);

        }
        #endregion GetUserMethods
        public User RegistrateUser(UserRegistrationData registrationData, Client current_client)
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
                return null;
            }
            reader.Close();

            command = new MySqlCommand($"INSERT INTO Users(user_name,user_login,user_password,user_email)" +
            	$"VALUES('{registrationData.Username}','{registrationData.Login}','{registrationData.Password}','{registrationData.Email}');",connection);

            try
            {
                command.ExecuteNonQuery();
                connection.Close();
                return GetUser(registrationData.Login, current_client);
            }
            catch(Exception ex)
            {
                if (ex.Message.Contains("user_email"))
                    Console.WriteLine("Email is taken : " + registrationData.Email);
                if (ex.Message.Contains("user_name"))
                    Console.WriteLine("Incorrect username : " + registrationData.Email);
                if (ex.Message.Contains("user_password"))
                    Console.WriteLine("Incorrect password : " + registrationData.Email);
                if (ex.Message.Contains("user_login"))
                    Console.WriteLine("Login is taken : " + registrationData.Email);
            }

            reader.Close();
            connection.Close();
            return null;
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

                using (connection)
                {

                    connection.Open();
                    var command = new MySqlCommand($"INSERT INTO Sessions(session_id,user_id,client_id) VALUES('{sid}',{user.Id},{user.Current_Client.Id});", connection);
                    command.ExecuteNonQuery();
                    connection.Close();

                }
                Console.WriteLine($"Session with {sid} has been created");




            
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
            string sid = "SID:" + user.Login + Convert.ToString((user.Id+1419)*DateTime.Now.Second+ user.Login.Length);
            
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

        public Client GetClient(string ip)
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
                        result = new Client(ip, reader.GetInt16(0));


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

    }
}


