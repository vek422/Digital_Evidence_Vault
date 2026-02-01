using System.Data;
using Microsoft.Data.Sqlite;

public class AuditRepository
{
    private AuditLog MapRowToAuditLog(DataRow data)
    {
        return new AuditLog(
            Convert.ToInt64(data["id"]),
            Convert.ToInt32(data["user_id"]),
            data["action"].ToString()!,
            DateTime.Parse(data["timestamp"].ToString()!),
            data["details"]?.ToString() ?? ""
        );
    }

    public void LogAction(int userId, LogAction action, string details = "")
    {
        string sql = @"
            INSERT INTO AuditLogs (user_id, action, timestamp, details)
            VALUES (@userId, @action, @timestamp, @details)";

        var parameters = new[]
        {
            new SqliteParameter("@userId", userId),
            new SqliteParameter("@action", action.ToString()),
            new SqliteParameter("@timestamp", DateTime.UtcNow.ToString("o")),
            new SqliteParameter("@details", details)
        };

        DatabaseHelper.ExecuteNonQuery(sql, parameters);
    }

    public List<AuditLog> GetLogsByUserId(int userId)
    {
        string sql = "SELECT * FROM AuditLogs WHERE user_id = @userId ORDER BY timestamp DESC";
        var parameters = new[] { new SqliteParameter("@userId", userId) };

        DataTable table = DatabaseHelper.ExecuteQuery(sql, parameters);
        var logs = new List<AuditLog>();

        foreach (DataRow row in table.Rows)
        {
            logs.Add(MapRowToAuditLog(row));
        }

        return logs;
    }

    public List<AuditLog> GetAllLogs(int limit = 100)
    {
        string sql = $"SELECT * FROM AuditLogs ORDER BY timestamp DESC LIMIT {limit}";
        DataTable table = DatabaseHelper.ExecuteQuery(sql);
        var logs = new List<AuditLog>();

        foreach (DataRow row in table.Rows)
        {
            logs.Add(MapRowToAuditLog(row));
        }

        return logs;
    }

    public List<AuditLog> GetLogsByAction(LogAction action)
    {
        string sql = "SELECT * FROM AuditLogs WHERE action = @action ORDER BY timestamp DESC";
        var parameters = new[] { new SqliteParameter("@action", action.ToString()) };

        DataTable table = DatabaseHelper.ExecuteQuery(sql, parameters);
        var logs = new List<AuditLog>();

        foreach (DataRow row in table.Rows)
        {
            logs.Add(MapRowToAuditLog(row));
        }

        return logs;
    }
}