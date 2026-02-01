public class AuditorMenu
{
    private readonly VerificationService _verificationService;

    public AuditorMenu()
    {
        _verificationService = new VerificationService();
    }

    public void VerifyEvidence()
    {
        Console.Clear();
        ConsoleHelper.PrintLogo();
        ConsoleHelper.PrintHeader("Verify Evidence Integrity");

        string evidenceId = ConsoleHelper.GetInput("Enter Evidence ID");

        if (string.IsNullOrWhiteSpace(evidenceId))
        {
            ConsoleHelper.PrintError("Evidence ID cannot be empty.");
            ConsoleHelper.Pause();
            return;
        }

        // Get evidence details first
        var (ledger, metadata) = _verificationService.GetEvidenceDetails(evidenceId);

        if (ledger == null || metadata == null)
        {
            ConsoleHelper.PrintError("Evidence not found in the system.");
            ConsoleHelper.Pause();
            return;
        }

        // Show evidence details
        Console.WriteLine();
        ConsoleHelper.PrintSubHeader("Evidence Details");
        Console.WriteLine($"  Evidence ID    : {ledger.ID}");
        Console.WriteLine($"  Original File  : {metadata.OriginalFileName}");
        Console.WriteLine($"  Case Number    : {metadata.CaseNumber ?? "N/A"}");
        Console.WriteLine($"  Description    : {metadata.Description ?? "N/A"}");
        Console.WriteLine($"  Uploaded       : {new DateTime(ledger.CreatedTicks):yyyy-MM-dd HH:mm:ss UTC}");
        Console.WriteLine($"  File Extension : {metadata.FileExtension}");
        Console.WriteLine();

        if (!ConsoleHelper.Confirm("Proceed with verification?"))
        {
            ConsoleHelper.PrintWarning("Verification cancelled.");
            ConsoleHelper.Pause();
            return;
        }

        Console.WriteLine();

        VerificationResult result = null!;

        ConsoleHelper.PrintSpinner("Verifying evidence integrity...", () =>
        {
            result = _verificationService.VerifyEvidence(evidenceId);
            Thread.Sleep(500); // Brief delay for visual effect
        });

        ConsoleHelper.PrintVerificationResult(result);

        ConsoleHelper.Pause();
    }

    public void ListAllEvidence()
    {
        Console.Clear();
        ConsoleHelper.PrintLogo();
        ConsoleHelper.PrintHeader("All Evidence in System");

        var evidence = _verificationService.GetAllEvidence();

        if (evidence.Count == 0)
        {
            ConsoleHelper.PrintInfo("No evidence found in the system.");
        }
        else
        {
            string[] headers = { "Evidence ID", "Original File", "Case #", "Extension", "Date Uploaded" };
            var rows = evidence.Select(e => new[]
            {
                e.Ledger.ID,
                e.Metadata.OriginalFileName.Length > 22 
                    ? e.Metadata.OriginalFileName.Substring(0, 19) + "..." 
                    : e.Metadata.OriginalFileName,
                e.Metadata.CaseNumber ?? "N/A",
                e.Metadata.FileExtension,
                new DateTime(e.Ledger.CreatedTicks).ToString("yyyy-MM-dd HH:mm")
            }).ToList();

            ConsoleHelper.PrintTable(headers, rows);

            Console.WriteLine();
            ConsoleHelper.PrintInfo($"Total: {evidence.Count} evidence file(s) in vault");
        }

        Console.WriteLine();
        ConsoleHelper.PrintInfo("To verify an evidence file, use option 1 and enter the Evidence ID.");
        ConsoleHelper.Pause();
    }
}
