using System;
using MySql.Data.MySqlClient;
using SLIVA.Models;
namespace SLIVA.Data
{
    public static class MySqlManager
    {
        static MySqlConnection connection = new MySqlConnection("Server=127.0.0.1;UserId=root;Password=55665566;Database=SLIVA_db;");
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
        public static bool UserExists(string login)
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

    }
}

