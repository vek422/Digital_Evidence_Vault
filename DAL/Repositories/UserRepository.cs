using System.Data;
using Microsoft.Data.Sqlite;

public class UserRepository
{
    private User MapRowToUser(DataRow data)
    {
        return new User
        (
            Convert.ToInt32(data["id"]),
            data["username"].ToString()!,
            data["password_hash"].ToString()!,
            data["salt"].ToString()!,
            (UserRole)Convert.ToInt32(data["role"]),
            data["name"].ToString()!
        );
    }

    public User? GetAdminAccount()
    {
        string sql = "SELECT * FROM User WHERE role = @r LIMIT 1";

        var parameters = new[]
        {
            new SqliteParameter("@r", (int)UserRole.Admin)
        };

        DataTable data = DatabaseHelper.ExecuteQuery(sql, parameters);

        if (data.Rows.Count == 0)
        {
            return null;
        }
        return MapRowToUser(data.Rows[0]);
    }

    public void CreateUserAccount(User user)
    {
        string sql = @"
            INSERT INTO User (username, password_hash, salt, role, name) 
            VALUES (@u, @p, @s, @r, @n)";

        var parameters = new[]
        {
            new SqliteParameter("@u", user.Username),
            new SqliteParameter("@p", user.PasswordHash),
            new SqliteParameter("@s", user.Salt),
            new SqliteParameter("@r", (int)user.Role),
            new SqliteParameter("@n", user.Name)
        };

        DatabaseHelper.ExecuteNonQuery(sql, parameters);
    }

    public User? GetByUserName(string username)
    {
        string sql = "SELECT * FROM User WHERE username = @u";

        var parameters = new[]
        {
            new SqliteParameter("@u", username)
        };

        DataTable table = DatabaseHelper.ExecuteQuery(sql, parameters);

        if (table.Rows.Count == 0) return null;
        return MapRowToUser(table.Rows[0]);
    }

    public List<User> GetAllUsers()
    {
        string sql = "SELECT * FROM User ORDER BY id";
        DataTable table = DatabaseHelper.ExecuteQuery(sql);

        var users = new List<User>();
        foreach (DataRow row in table.Rows)
        {
            users.Add(MapRowToUser(row));
        }
        return users;
    }

    public bool UsernameExists(string username)
    {
        string sql = "SELECT COUNT(*) FROM User WHERE username = @u";
        var parameters = new[]
        {
            new SqliteParameter("@u", username)
        };

        var result = DatabaseHelper.ExecuteScalar(sql, parameters);
        return Convert.ToInt32(result) > 0;
    }
}