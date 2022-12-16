using System;
using MySql.Data.MySqlClient;
using SLIVA.Models;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using System.Linq;
namespace SLIVA.Data
{
    public class MySqlManager
    {
        static MySqlConnection connection = new MySqlConnection("Server=127.0.0.1;UserId=root;Password=55665566;Database=SLIVA_db;");
        #region User-related
        #region GetUserMethods
        public User GetUser(UserAuthenticationData auth_data)
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

                return new User(user_name, user_login, user_password, user_email, user_id);
            }

            reader.Close();
            connection.Close();
            return null;

        }
        public User GetUser(string login)
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

                return new User(user_name, user_login, user_password, user_email, user_id);

        }
        #endregion GetUserMethods
        public User RegistrateUser(UserRegistrationData registrationData)
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
                return GetUser(registrationData.Login);
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

        public void CreateSession(User user)
        {
            //using (connection)
            //{
            //    connection.Open();
            //    var command = new MySqlCommand($"INSERT INTO Sessions(session_id,user_id,client_id,session_expires,session_expires_at) VALUES('test_ssid1',1,1,1,'2023-10-23 12:45:37.123');", connection);

            //}
            Console.WriteLine("Creating session (kinda)");
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
                        Console.WriteLine(reader.GetName(i) + " : " + GetIpAddressString((reader[i] as byte[])));
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
                byte[] binary_ip = GetIpAddressBinary(ip);
                using (connection)
                {
                    connection.Open();
                    var command = new MySqlCommand($"INSERT INTO Clients(client_ip) VALUES(@binary_ip)", connection);
                    command.Parameters.Add("@binary_ip", MySqlDbType.Blob).Value = binary_ip;
                    command.ExecuteNonQuery();
                    connection.Close();
                }

            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex);
            }
        }

        public bool ClientExists(string ip)
        {
            try
            {
                byte[] binary_ip = GetIpAddressBinary(ip);
                using (connection)
                {
                    connection.Open();

                    var command = new MySqlCommand($"SELECT * FROM Clients WHERE client_ip = @binary_ip;", connection);
                    command.Parameters.Add("@binary_ip", MySqlDbType.Blob).Value = binary_ip;
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Close();
                        connection.Close();
                        return true;
                    }
                    reader.Close();
                    connection.Close();
                    return false;
                }
            }
            catch (MySqlException)
            {
                connection.Close();
                return false;
            }
        }
            #endregion
            private byte[] GetIpAddressBinary(string Ip)
        {
            IPAddress IPDec = IPAddress.Parse(Ip);
            byte[] IPByte = IPDec.GetAddressBytes();
            return IPByte;

        }
        private string GetIpAddressString(byte[] binary_ip)
        {
            string result = "";
            foreach (byte b in binary_ip)
            {
                result += b + ".";
            }
            result = result.TrimEnd(".".ToCharArray());
            Console.WriteLine(result);
            return result;
        }
    }
}


