using System.Data;
using System.Xml;
using MySqlConnector;

public class UserRepository()
{

    private User MapRowToUser(DataRow data)
    {
        return new User
        (
            Convert.ToInt32(data["id"]),
            data["username"].ToString(),
            data["password_hash"].ToString(),
            data["salt"].ToString(),
            (UserRole)Convert.ToInt32(data["role"]),
            data["name"].ToString()
        );
    }
    public User? GetAdminAccount()
    {
        string SqlQuery = "SELECT * FROM User Where Role = @r LIMIT 1";

        var parameters = new[]
        {
            new MySqlParameter("@r",(int)UserRole.Admin)
        };
        DataTable data = DatabaseHelper.ExecuteQuery(SqlQuery, parameters);

        if (data.Rows.Count == 0)
        {
            return null;
        }
        return MapRowToUser(data.Rows[0]);
    }

    public void CreateUserAccount(User user)
    {
        string sql = @"
        INSERT INTO User (username, password_hash, salt, role, name) VALUES (@u, @p, @s, @r, @n)";

        var parameters = new[]
        {
            new MySqlParameter("@u", user.Username),
            new MySqlParameter("@p", user.PasswordHash),
            new MySqlParameter("@s",user.Salt),
            new MySqlParameter("@r", user.Role),
            new MySqlParameter("@n", user.Name)
        };

        DatabaseHelper.ExecuterNonQuery(sql, parameters);
    }

    public User? GetByUserName(string username)
    {
        string sql = @"SELECT * FROM user WHERE username = @u";

        var parameters = new[]
        {
            new MySqlParameter("@u",username)
        };
        DataTable table = DatabaseHelper.ExecuteQuery(sql, parameters);

        if (table.Rows.Count == 0) return null;
        return MapRowToUser(table.Rows[0]);
    }
}