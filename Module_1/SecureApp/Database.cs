using System;
using System.Data.SqlClient;
//using System.Security.Cryptography;
//using System.Text;
using MySql.Data.MySqlClient;

public class Database
{
    private readonly string _connectionString = "Data Source=server;Initial Catalog=SecureApp;Integrated Security=True";

    public bool AuthenticateUser(String username, String password)
    {
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {
            string query = "SELECT * FROM Users WHERE Username = @username AND Password = @password";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);

            conn.Open();
            MySqlDataReader reader = cmd.ExecuteReader();
            return reader.HasRows;
        }
    }
    
}