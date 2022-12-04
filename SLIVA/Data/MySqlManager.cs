using System;
using MySql.Data.MySqlClient;
using SLIVA.Models;
using System.Threading.Tasks;
namespace SLIVA.Data
{
    public class MySqlManager
    {
        static MySqlConnection connection = new MySqlConnection("Server=127.0.0.1;UserId=root;Password=55665566;Database=SLIVA_db;");
        #region User-related
        public static User GetUser(UserAuthenticationData auth_data)
        {
            connection.Open();
            var command = new MySqlCommand($"SELECT * FROM Users WHERE user_login = '{auth_data.Login}';",connection);
            var reader = command.ExecuteReader();
            reader.Read();
            if (!reader.HasRows)
            {

                reader.Close();
                connection.Close();
                throw new AggregateException("User does not exist!");
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
            connection.Close();
            reader.Close();

            return false;
        }
        #endregion
        #region Client-related
        public void ConsoleWriteClients()
        {
            using (connection)
            {
                connection.Open();

                int row_count = Convert.ToInt32(new MySqlCommand("SELECT COUNT(user_id) FROM Users", connection).ExecuteScalar());

                Console.WriteLine("Row amount : " + row_count);

                var command = new MySqlCommand("SELECT * FROM Users", connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    for (int i = reader.FieldCount - 1; i > 0; i--)
                        Console.WriteLine(reader.GetName(i) + " : " + reader[i]);
                    Console.WriteLine();
                }
                connection.Close();
            }
        }

        public void InsertClientIpIntoDatabase(byte[] binary_ip)
        {
            try
            {
                using (connection)
                {
                    connection.Open();
                    var command = new MySqlCommand($"INSERT INTO Clients(IpAddress) VALUES(@binary_ip)", connection);
                    command.Parameters.Add("@binary_ip", MySqlDbType.Blob).Value = binary_ip;
                    command.ExecuteNonQuery();
                    connection.Close();
                }

            }
            catch (MySqlException)
            {
                connection.Close();
            }
        }

        public bool IsDatabaseCointainsClient(byte[] binary_ip)
        {
            try
            {
                using (connection)
                {
                    connection.Open();

                    var command = new MySqlCommand($"SELECT * FROM Clients WHERE IpAddress = @binary_ip;", connection);
                    command.Parameters.Add("@binary_ip", MySqlDbType.Blob).Value = binary_ip;

                    if (command.ExecuteReader().HasRows)
                    {
                        connection.Close();
                        return true;
                    }
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
    }
}


