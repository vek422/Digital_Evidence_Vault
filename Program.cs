using System;

public class Program
{
    public static void Main()
    {
        while (true)
        {
            PrepareScreen();
            var user = AuthenticationUI.LoginScreen();
            MenuRouter.PrintMenu(user);
            Console.WriteLine("Logging Out");
            Thread.Sleep(1000);
        }
    }

    static void PrepareScreen()
    {

    }
}