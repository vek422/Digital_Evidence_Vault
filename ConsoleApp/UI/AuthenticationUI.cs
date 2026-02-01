using System.Text;

public static class AuthenticationUI
{
    private static readonly AuthService _authService = new AuthService();

    public static User? LoginScreen()
    {
        const int maxAttempts = 3;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            Console.WriteLine("════════════════════════════════════════");
            Console.WriteLine("  Please Enter Your Credentials to Login");
            Console.WriteLine("════════════════════════════════════════");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("  Username : ");
            Console.ResetColor();
            string? username = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(username))
            {
                ConsoleHelper.PrintError("Username cannot be empty.");
                attempts++;
                continue;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("  Password : ");
            Console.ResetColor();
            string password = PasswordInput();

            User? user = _authService.Login(username, password);

            if (user != null)
            {
                Console.WriteLine();
                ConsoleHelper.PrintSuccess($"Welcome, {user.Name}!");
                Console.WriteLine();
                Thread.Sleep(1000);
                return user;
            }

            attempts++;
            Console.WriteLine();
            ConsoleHelper.PrintError($"Invalid credentials. Attempts remaining: {maxAttempts - attempts}");
            Console.WriteLine();

            if (attempts < maxAttempts)
            {
                Thread.Sleep(1000);
            }
        }

        ConsoleHelper.PrintError("Maximum login attempts exceeded. Please try again later.");
        Thread.Sleep(2000);
        return null;
    }

    private static string PasswordInput()
    {
        var password = new StringBuilder();

        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Backspace)
            {
                if (password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b");
                }
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
}