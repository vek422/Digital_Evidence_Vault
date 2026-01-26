public class MenuRouter()
{

    public static void PrintMenu(User user)
    {
        switch ((UserRole)user.Role)
        {
            case UserRole.Admin:
                {
                    RunAdminMenu(user);
                    break;
                }
            case UserRole.Auditor:
                {
                    RunAuditorMenu(user);
                    break;
                }
            case UserRole.Custodian:
                {
                    RunCustodianMenu(user);
                    break;
                }
        }

    }

    static void RunAuditorMenu(User user)
    {
        while (true)
        {
        }
    }

    static void RunAdminMenu(User user)
    {
        while (true)
        { }
    }

    static void RunCustodianMenu(User user)
    {
        while (true) { }
    }
}