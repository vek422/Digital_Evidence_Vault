
public class User
{
    public int ID { get; init; }
    public string Username { get; init; }
    public string PasswordHash { get; set; }
    public string Salt { get; set; }
    public int Role { get; init; }

    public User(int ID, string Username, string PasswordHash, string Salt, int Role)
    {
        this.ID = ID;
        this.Username = Username;
        this.PasswordHash = PasswordHash;
        this.Salt = Salt;
        this.Role = Role;
    }
    public User()
    {

    }

}