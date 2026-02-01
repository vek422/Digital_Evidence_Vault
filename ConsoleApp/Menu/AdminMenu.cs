public class AdminMenu
{
    private readonly AdminService _adminService;

    public AdminMenu()
    {
        _adminService = new AdminService();
    }

    public void CreateUser()
    {
        Console.Clear();
        ConsoleHelper.PrintLogo();
        ConsoleHelper.PrintHeader("Create New User");

        string username = ConsoleHelper.GetInput("Username");
        var userRepo = new UserRepository();
        if (userRepo.UsernameExists(username))
        {
            ConsoleHelper.PrintError("Username already exists. Please choose a different username.");
            ConsoleHelper.Pause();
            return;
        }

        string? password = GetPasswordWithConfirmation();
        if (password == null)
        {
            return;
        }

        string name = ConsoleHelper.GetInput("Full Name");

        ConsoleHelper.PrintSubHeader("Select Role");
        Console.WriteLine("  [1] Admin");
        Console.WriteLine("  [2] Custodian");
        Console.WriteLine("  [3] Auditor");
        Console.WriteLine();

        int roleChoice = ConsoleHelper.GetMenuChoice(3);
        UserRole role = roleChoice switch
        {
            1 => UserRole.Admin,
            2 => UserRole.Custodian,
            3 => UserRole.Auditor,
            _ => UserRole.Auditor
        };

        Console.WriteLine();
        Console.WriteLine("  Creating user with the following details:");
        Console.WriteLine($"    Username : {username}");
        Console.WriteLine($"    Name     : {name}");
        Console.WriteLine($"    Role     : {role}");
        Console.WriteLine();

        if (!ConsoleHelper.Confirm("Confirm user creation?"))
        {
            ConsoleHelper.PrintWarning("User creation cancelled.");
            ConsoleHelper.Pause();
            return;
        }

        bool success = _adminService.CreateUser(username, password, name, role);

        Console.WriteLine();
        if (success)
        {
            ConsoleHelper.PrintSuccess($"User '{username}' created successfully!");
        }
        else
        {
            ConsoleHelper.PrintError("Failed to create user.");
        }

        ConsoleHelper.Pause();
    }

    private string? GetPasswordWithConfirmation()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Password: ");
        Console.ResetColor();
        string password = ReadPassword();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Confirm Password: ");
        Console.ResetColor();
        string confirmPassword = ReadPassword();

        if (password != confirmPassword)
        {
            ConsoleHelper.PrintError("Passwords do not match.");
            ConsoleHelper.Pause();
            return null;
        }

        if (password.Length < 6)
        {
            ConsoleHelper.PrintError("Password must be at least 6 characters long.");
            ConsoleHelper.Pause();
            return null;
        }

        return password;
    }

    private string ReadPassword()
    {
        var password = new System.Text.StringBuilder();
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password.Remove(password.Length - 1, 1);
                Console.Write("\b \b");
            }
            else if (key.Key != ConsoleKey.Enter && !char.IsControl(key.KeyChar))
            {
                password.Append(key.KeyChar);
                Console.Write("*");
            }
        } while (key.Key != ConsoleKey.Enter);
        Console.WriteLine();
        return password.ToString();
    }

    public void ViewAllUsers()
    {
        Console.Clear();
        ConsoleHelper.PrintLogo();
        ConsoleHelper.PrintHeader("All Users");

        var users = _adminService.GetAllUsers();

        if (users.Count == 0)
        {
            ConsoleHelper.PrintInfo("No users found.");
        }
        else
        {
            string[] headers = { "ID", "Username", "Name", "Role" };
            var rows = users.Select(u => new[]
            {
                u.ID.ToString(),
                u.Username,
                u.Name,
                u.Role.ToString()
            }).ToList();

            ConsoleHelper.PrintTable(headers, rows);
        }

        ConsoleHelper.Pause();
    }

    public void ViewAuditLogs()
    {
        Console.Clear();
        ConsoleHelper.PrintLogo();
        ConsoleHelper.PrintHeader("Audit Logs (Last 50)");

        var logs = _adminService.GetAuditLogs(50);

        if (logs.Count == 0)
        {
            ConsoleHelper.PrintInfo("No audit logs found.");
        }
        else
        {
            string[] headers = { "ID", "User ID", "Action", "Timestamp", "Details" };
            var rows = logs.Select(l => new[]
            {
                l.ID.ToString(),
                l.UserId.ToString(),
                l.Action,
                l.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss"),
                l.Details.Length > 40 ? l.Details.Substring(0, 37) + "..." : l.Details
            }).ToList();

            ConsoleHelper.PrintTable(headers, rows);
        }

        ConsoleHelper.Pause();
    }
}
