public class AuthService
{
    private readonly UserRepository _userRepo;
    private readonly AuditRepository _auditRepo;

    public AuthService()
    {
        _userRepo = new UserRepository();
        _auditRepo = new AuditRepository();
    }

    public User? Login(string username, string password)
    {
        User? user = _userRepo.GetByUserName(username);

        if (user == null)
        {
            return null;
        }

        string computedHash = PasswordHelper.HashPassword(password, user.Salt);

        if (!string.Equals(computedHash, user.PasswordHash, StringComparison.OrdinalIgnoreCase))
        {
            _auditRepo.LogAction(user.ID, LogAction.AccessDenied, $"Failed login attempt for user: {username}");
            return null;
        }

        _auditRepo.LogAction(user.ID, LogAction.Login, $"User {username} logged in successfully");
        return user;
    }

    public void Logout(User user)
    {
        _auditRepo.LogAction(user.ID, LogAction.Logout, $"User {user.Username} logged out");
    }
}
