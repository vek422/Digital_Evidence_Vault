using System.Data;

public static class ConsoleHelper
{
    public static void PrintLogo()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine(@"                                                     
████▄  ██  ▄████  ██ ██████ ▄████▄ ██       ██████ ██  ██ ██ ████▄  ██████ ███  ██ ▄█████ ██████   ██  ██ ▄████▄ ██  ██ ██    ██████ 
██  ██ ██ ██  ▄▄▄ ██   ██   ██▄▄██ ██       ██▄▄   ██▄▄██ ██ ██  ██ ██▄▄   ██ ▀▄██ ██     ██▄▄     ██▄▄██ ██▄▄██ ██  ██ ██      ██   
████▀  ██  ▀███▀  ██   ██   ██  ██ ██████   ██▄▄▄▄  ▀██▀  ██ ████▀  ██▄▄▄▄ ██   ██ ▀█████ ██▄▄▄▄    ▀██▀  ██  ██ ▀████▀ ██████  ██   
                                                                                           ");
        Console.ResetColor();
    }

    public static void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{message}");
        Console.ResetColor();
    }

    public static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{message}");
        Console.ResetColor();
    }

    public static void PrintWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"⚠ {message}");
        Console.ResetColor();
    }

    public static void PrintInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"ℹ {message}");
        Console.ResetColor();
    }

    public static void PrintHeader(string title)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔" + new string('═', title.Length + 2) + "╗");
        Console.WriteLine($"║ {title} ║");
        Console.WriteLine("╚" + new string('═', title.Length + 2) + "╝");
        Console.ResetColor();
        Console.WriteLine();
    }

    public static void PrintSubHeader(string title)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"─── {title} ───");
        Console.ResetColor();
    }

    public static void PrintMenu(string title, string[] options)
    {
        PrintHeader(title);
        for (int i = 0; i < options.Length; i++)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"  [{i + 1}] ");
            Console.ResetColor();
            Console.WriteLine(options[i]);
        }
        Console.WriteLine();
    }

    public static int GetMenuChoice(int maxOption)
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter your choice: ");
            Console.ResetColor();

            string? input = Console.ReadLine();
            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= maxOption)
            {
                return choice;
            }
            PrintError($"Invalid choice. Please enter a number between 1 and {maxOption}.");
        }
    }

    public static string GetInput(string prompt, bool required = true)
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{prompt}: ");
            Console.ResetColor();

            string? input = Console.ReadLine()?.Trim();

            if (!required || !string.IsNullOrEmpty(input))
            {
                return input ?? "";
            }
            PrintError("This field is required.");
        }
    }

    public static bool Confirm(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"{message} (Y/N): ");
        Console.ResetColor();

        var key = Console.ReadKey(true);
        Console.WriteLine(key.KeyChar);

        return key.Key == ConsoleKey.Y;
    }

    public static void PrintTable(string[] headers, List<string[]> rows)
    {
        if (rows.Count == 0)
        {
            PrintInfo("No data to display.");
            return;
        }

        // Calculate column widths
        int[] widths = new int[headers.Length];
        for (int i = 0; i < headers.Length; i++)
        {
            widths[i] = headers[i].Length;
        }
        foreach (var row in rows)
        {
            for (int i = 0; i < row.Length && i < widths.Length; i++)
            {
                widths[i] = Math.Max(widths[i], row[i]?.Length ?? 0);
            }
        }

        // Print header
        Console.ForegroundColor = ConsoleColor.Cyan;
        PrintTableRow(headers, widths);
        PrintTableSeparator(widths);
        Console.ResetColor();

        // Print rows
        foreach (var row in rows)
        {
            PrintTableRow(row, widths);
        }

        Console.WriteLine();
    }

    private static void PrintTableRow(string[] columns, int[] widths)
    {
        Console.Write("│ ");
        for (int i = 0; i < columns.Length; i++)
        {
            string value = columns[i] ?? "";
            Console.Write(value.PadRight(widths[i]));
            Console.Write(" │ ");
        }
        Console.WriteLine();
    }

    private static void PrintTableSeparator(int[] widths)
    {
        Console.Write("├─");
        for (int i = 0; i < widths.Length; i++)
        {
            Console.Write(new string('─', widths[i]));
            Console.Write(i < widths.Length - 1 ? "─┼─" : "─┤");
        }
        Console.WriteLine();
    }

    public static void PrintSpinner(string message, Action action)
    {
        var spinnerChars = new[] { '|', '/', '-', '\\' };
        int spinnerIndex = 0;
        bool completed = false;

        var spinnerTask = Task.Run(() =>
        {
            while (!completed)
            {
                Console.Write($"\r{spinnerChars[spinnerIndex]} {message}");
                spinnerIndex = (spinnerIndex + 1) % spinnerChars.Length;
                Thread.Sleep(100);
            }
        });

        action();
        completed = true;
        spinnerTask.Wait();

        Console.WriteLine($"\r {message}");
    }

    public static void ClearLine()
    {
        Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
    }

    public static void Pause(string message = "Press any key to continue...")
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(message);
        Console.ResetColor();
        Console.ReadKey(true);
    }

    public static void PrintVerificationResult(VerificationResult result)
    {
        Console.WriteLine();
        if (result.IsAuthentic)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("╔════════════════════════════════════════════╗");
            Console.WriteLine("║          EVIDENCE IS AUTHENTIC             ║");
            Console.WriteLine("╚════════════════════════════════════════════╝");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("╔════════════════════════════════════════════╗");
            Console.WriteLine("║        EVIDENCE VERIFICATION FAILED        ║");
            Console.WriteLine("╚════════════════════════════════════════════╝");
        }
        Console.ResetColor();

        Console.WriteLine();
        Console.WriteLine($"  Evidence ID     : {result.EvidenceId}");
        Console.Write("  Database Status : ");
        PrintStatus(result.IsDatabaseIntact, "Intact", "TAMPERED");
        Console.Write("  File Status     : ");
        PrintStatus(result.IsFileIntact, "Intact", "CORRUPTED");
        Console.WriteLine();
        Console.WriteLine($"  Message: {result.Message}");
        Console.WriteLine();
    }

    private static void PrintStatus(bool isOk, string okText, string failText)
    {
        if (isOk)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{okText}");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{failText}");
        }
        Console.ResetColor();
    }
}
