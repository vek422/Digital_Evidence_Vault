public static class MenuRouter
{
    public static void PrintMenu(User user)
    {
        SessionManager.SetUser(user);

        switch (user.Role)
        {
            case UserRole.Admin:
                RunAdminMenu(user);
                break;
            case UserRole.Auditor:
                RunAuditorMenu(user);
                break;
            case UserRole.Custodian:
                RunCustodianMenu(user);
                break;
        }

        SessionManager.ClearSession();
    }

    private static void RunAdminMenu(User user)
    {
        var adminMenu = new AdminMenu();
        bool running = true;

        while (running)
        {
            Console.Clear();
            ConsoleHelper.PrintLogo();
            ConsoleHelper.PrintInfo($"Logged in as: {user.Name} (Administrator)");

            string[] options = {
                "Create New User",
                "View All Users",
                "View Audit Logs",
                "Logout"
            };

            ConsoleHelper.PrintMenu("Admin Dashboard", options);
            int choice = ConsoleHelper.GetMenuChoice(options.Length);

            switch (choice)
            {
                case 1:
                    adminMenu.CreateUser();
                    break;
                case 2:
                    adminMenu.ViewAllUsers();
                    break;
                case 3:
                    adminMenu.ViewAuditLogs();
                    break;
                case 4:
                    running = false;
                    break;
            }
        }
    }

    private static void RunCustodianMenu(User user)
    {
        var custodianMenu = new CustodianMenu();
        bool running = true;

        while (running)
        {
            Console.Clear();
            ConsoleHelper.PrintLogo();
            ConsoleHelper.PrintInfo($"Logged in as: {user.Name} (Custodian)");

            string[] options = {
                "Upload Evidence",
                "View My Uploads",
                "Logout"
            };

            ConsoleHelper.PrintMenu("Custodian Dashboard", options);
            int choice = ConsoleHelper.GetMenuChoice(options.Length);

            switch (choice)
            {
                case 1:
                    custodianMenu.UploadEvidence();
                    break;
                case 2:
                    custodianMenu.ViewMyUploads();
                    break;
                case 3:
                    running = false;
                    break;
            }
        }
    }

    private static void RunAuditorMenu(User user)
    {
        var auditorMenu = new AuditorMenu();
        bool running = true;

        while (running)
        {
            Console.Clear();
            ConsoleHelper.PrintLogo();
            ConsoleHelper.PrintInfo($"Logged in as: {user.Name} (Auditor)");

            string[] options = [
                "Verify Evidence by ID",
                "List All Evidence",
                "Logout"
            ];

            ConsoleHelper.PrintMenu("Auditor Dashboard", options);
            int choice = ConsoleHelper.GetMenuChoice(options.Length);

            switch (choice)
            {
                case 1:
                    auditorMenu.VerifyEvidence();
                    break;
                case 2:
                    auditorMenu.ListAllEvidence();
                    break;
                case 3:
                    running = false;
                    break;
            }
        }
    }
}