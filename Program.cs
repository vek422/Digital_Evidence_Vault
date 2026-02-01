public class Program
{
    public static void Main()
    {
        try
        {
            DatabaseHelper.InitializeDatabase();
            var adminService = new AdminService();
            adminService.CreateDefaultAdminAccount();

            while (true)
            {
                Console.Clear();
                PrepareScreen();

                var user = AuthenticationUI.LoginScreen();

                if (user != null)
                {
                    MenuRouter.PrintMenu(user);
                    var authService = new AuthService();
                    authService.Logout(user);


                    Console.Clear();
                    ConsoleHelper.PrintLogo();
                    ConsoleHelper.PrintInfo("Logging out...");
                    Thread.Sleep(1000);
                }
                else
                {
                    // Failed login - wait before allowing retry
                    Thread.Sleep(2000);
                }
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║         APPLICATION ERROR              ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine($"\nError: {ex.Message}");
            Console.WriteLine($"\nStack Trace:\n{ex.StackTrace}");
            Console.ResetColor();
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey(true);
        }
    }

    static void PrepareScreen()
    {
        ConsoleHelper.PrintLogo();
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("  Secure Digital Evidence Management System");
        Console.WriteLine("  Ensuring Chain of Custody with Cryptographic Integrity");
        Console.ResetColor();
        Console.WriteLine();
    }
}