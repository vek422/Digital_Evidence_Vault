public class User
{
    public int ID { get; init; }
    public string Username { get; init; }
    public string PasswordHash { get; set; }
    public string Salt { get; set; }
    public UserRole Role { get; init; }
    public string Name { get; set; }

    public User(int ID, string Username, string PasswordHash, string Salt, UserRole Role, string Name)
    {
        this.Name = Name;
        this.ID = ID;
        this.Username = Username;
        this.PasswordHash = PasswordHash;
        this.Salt = Salt;
        this.Role = Role;
    }
}