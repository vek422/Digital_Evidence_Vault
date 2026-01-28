using System;

public class Program
{
    public static void Main()
    {
        Console.Clear();
        while (true)
        {
            PrepareScreen();
            var user = AuthenticationUI.LoginScreen();
            // MenuRouter.PrintMenu(user);
            Console.WriteLine("Logging Out");
            Thread.Sleep(1000);
        }
    }

    static void PrepareScreen()
    {

        ConsoleHelper.PrintLogo();

    }
}