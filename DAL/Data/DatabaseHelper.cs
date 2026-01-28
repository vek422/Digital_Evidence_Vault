using System.Data;
using Microsoft.VisualBasic;
using MySqlConnector;

public static class DatabaseHelper
{

    public static string ConnectionString { get; }
    static DatabaseHelper()
    {
        var builder = new MySqlConnectionStringBuilder
        {
            Server = "localhost",
            Database = "digital_vault",
            UserID = "admin",
            Password = "vek422",
            Port = 3306
        };
        ConnectionString = builder.ConnectionString;
    }

    public static DataTable ExecuteQuery(string query, MySqlParameter[]? parameters = null)
    {

        using var conn = new MySqlConnection(ConnectionString);
        using var command = new MySqlCommand(query, conn);
        using var adapter = new MySqlDataAdapter(command);
        {
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
    }

    public static object? ExecuteScaler(string query, MySqlParameter[]? parameters = null)
    {
        using var conn = new MySqlConnection(ConnectionString);
        using var cmd = new MySqlCommand(query);
        {
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }
            conn.Open();

            return cmd.ExecuteScalar();
        }
    }

    public static int ExecuterNonQuery(string query, MySqlParameter[]? parameters)
    {
        using var conn = new MySqlConnection(ConnectionString);
        using var cmd = new MySqlCommand(query, conn);
        {
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }
            conn.Open();
            return cmd.ExecuteNonQuery();

        }
    }
}