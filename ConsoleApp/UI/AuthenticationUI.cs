using System.Text;

public class AuthenticationUI
{
    public static User LoginScreen()
    {
        Console.WriteLine("======================================");
        Console.WriteLine("Please Enter Your Credentials to Login");
        Console.WriteLine("======================================");
        Console.WriteLine();
        Console.Write("Username : ");
        string username = Console.ReadLine();
        Console.Write("Password : ");
        string password = PasswordInput();
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
            else if (key.Key != ConsoleKey.Enter)
            {
                password.Append(key.KeyChar);
                Console.Write("*");
            }
        } while (key.Key != ConsoleKey.Enter);
        Console.WriteLine();
        return password.ToString();
    }
}