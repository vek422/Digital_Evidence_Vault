using System.Configuration;

public class AdminService
{
    private readonly UserRepository _userRepo;
    private readonly AuditRepository _auditRepo;

    public AdminService()
    {
        _userRepo = new UserRepository();
        _auditRepo = new AuditRepository();
    }

    public void CreateDefaultAdminAccount()
    {
        User? admin = _userRepo.GetAdminAccount();
        if (admin != null) return;

        string defaultPassword = ConfigurationManager.AppSettings["DefaultAdminPassword"] ?? "Admin@123";
        string salt = PasswordHelper.GenerateSalt();
        string passwordHash = PasswordHelper.HashPassword(defaultPassword, salt);

        var adminUser = new User(
            0,
            "admin",
            passwordHash,
            salt,
            UserRole.Admin,
            "System Administrator"
        );

        _userRepo.CreateUserAccount(adminUser);
    }

    public bool CreateUser(string username, string password, string name, UserRole role)
    {
        if (_userRepo.UsernameExists(username))
        {
            return false;
        }

        string salt = PasswordHelper.GenerateSalt();
        string passwordHash = PasswordHelper.HashPassword(password, salt);

        var user = new User(
            0,
            username,
            passwordHash,
            salt,
            role,
            name
        );

        _userRepo.CreateUserAccount(user);

        if (SessionManager.CurrentUser != null)
        {
            _auditRepo.LogAction(
                SessionManager.CurrentUser.ID,
                LogAction.UserCreated,
                $"Created user: {username} with role: {role}"
            );
        }

        return true;
    }

    public List<User> GetAllUsers()
    {
        return _userRepo.GetAllUsers();
    }

    public List<AuditLog> GetAuditLogs(int limit = 100)
    {
        return _auditRepo.GetAllLogs(limit);
    }
}