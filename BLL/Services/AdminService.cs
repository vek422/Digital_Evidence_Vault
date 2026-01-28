public class AdminService
{
    private UserRepository _userRepo;
    private AuditRepository _auditRepo;
    public AdminService()
    {
        _userRepo = new UserRepository();
        _auditRepo = new AuditRepository();
    }

    public void CreateAdminAccount()
    {
        // check if admin account exists already
        User? admin = _userRepo.GetAdminAccount();
        if (admin != null) return;


        // create admin account with default credentials
        admin = new User
        {
            Name = "admin",
            PasswordHash =
        }
    }
}