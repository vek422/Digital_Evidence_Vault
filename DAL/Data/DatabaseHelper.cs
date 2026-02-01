using System.Configuration;
using System.Data;
using Microsoft.Data.Sqlite;

public static class DatabaseHelper
{
    private static readonly string _databasePath;
    private static readonly string _connectionString;

    static DatabaseHelper()
    {
        _databasePath = ConfigurationManager.AppSettings["DatabasePath"] ?? "Aether.db";
        _connectionString = $"Data Source={_databasePath}";
    }

    public static string ConnectionString => _connectionString;

    public static void InitializeDatabase()
    {
        if (!File.Exists(_databasePath))
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string schemaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schema.sql");
            if (!File.Exists(schemaPath))
            {
                schemaPath = "schema.sql";
            }

            if (File.Exists(schemaPath))
            {
                string schema = File.ReadAllText(schemaPath);
                using var command = new SqliteCommand(schema, connection);
                command.ExecuteNonQuery();
            }
            else
            {
                CreateTablesManually(connection);
            }
        }
        else
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            CreateTablesManually(connection);
        }
    }

    private static void CreateTablesManually(SqliteConnection connection)
    {
        string createTables = @"
            CREATE TABLE IF NOT EXISTS User (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                username TEXT NOT NULL UNIQUE,
                password_hash TEXT NOT NULL,
                salt TEXT NOT NULL,
                role INTEGER NOT NULL,
                name TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS EvidenceLedger (
                id TEXT PRIMARY KEY,
                file_hash TEXT NOT NULL,
                integrity_signature TEXT NOT NULL,
                stored_file_name TEXT NOT NULL,
                uploader_id INTEGER NOT NULL,
                created_at_tick INTEGER NOT NULL,
                FOREIGN KEY (uploader_id) REFERENCES User(id)
            );

            CREATE TABLE IF NOT EXISTS EvidenceMetadata (
                id TEXT PRIMARY KEY,
                original_file_name TEXT NOT NULL,
                description TEXT,
                file_extension TEXT NOT NULL,
                case_number TEXT,
                FOREIGN KEY (id) REFERENCES EvidenceLedger(id)
            );

            CREATE TABLE IF NOT EXISTS AuditLogs (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                user_id INTEGER NOT NULL,
                action TEXT NOT NULL,
                timestamp TEXT NOT NULL,
                details TEXT,
                FOREIGN KEY (user_id) REFERENCES User(id)
            );
        ";

        using var command = new SqliteCommand(createTables, connection);
        command.ExecuteNonQuery();
    }

    public static DataTable ExecuteQuery(string query, SqliteParameter[]? parameters = null)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        using var command = new SqliteCommand(query, connection);

        if (parameters != null)
        {
            command.Parameters.AddRange(parameters);
        }

        var dataTable = new DataTable();
        using var reader = command.ExecuteReader();
        dataTable.Load(reader);
        return dataTable;
    }

    public static object? ExecuteScalar(string query, SqliteParameter[]? parameters = null)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        using var command = new SqliteCommand(query, connection);

        if (parameters != null)
        {
            command.Parameters.AddRange(parameters);
        }

        return command.ExecuteScalar();
    }

    public static int ExecuteNonQuery(string query, SqliteParameter[]? parameters = null)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        using var command = new SqliteCommand(query, connection);

        if (parameters != null)
        {
            command.Parameters.AddRange(parameters);
        }

        return command.ExecuteNonQuery();
    }

    public static SqliteConnection GetConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }
}