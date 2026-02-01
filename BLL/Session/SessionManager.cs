public static class SessionManager
{
    private static User? _currentUser;
    private static readonly object _lock = new object();

    public static User? CurrentUser
    {
        get
        {
            lock (_lock)
            {
                return _currentUser;
            }
        }
    }

    public static bool IsLoggedIn => CurrentUser != null;

    public static void SetUser(User user)
    {
        lock (_lock)
        {
            _currentUser = user;
        }
    }

    public static void ClearSession()
    {
        lock (_lock)
        {
            _currentUser = null;
        }
    }

    public static bool HasRole(UserRole role)
    {
        var user = CurrentUser;
        return user != null && user.Role == role;
    }

    public static bool IsAdmin => HasRole(UserRole.Admin);
    public static bool IsCustodian => HasRole(UserRole.Custodian);
    public static bool IsAuditor => HasRole(UserRole.Auditor);
}
