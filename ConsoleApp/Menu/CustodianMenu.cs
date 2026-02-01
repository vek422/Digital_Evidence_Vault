public class CustodianMenu
{
    private readonly EvidenceService _evidenceService;

    public CustodianMenu()
    {
        _evidenceService = new EvidenceService();
    }

    public void UploadEvidence()
    {
        Console.Clear();
        ConsoleHelper.PrintLogo();
        ConsoleHelper.PrintHeader("Upload Evidence");

        string filePath = ConsoleHelper.GetInput("File Path (full path to file)");

        if (!File.Exists(filePath))
        {
            ConsoleHelper.PrintError("File not found. Please check the path and try again.");
            ConsoleHelper.Pause();
            return;
        }

        // Show file info
        var fileInfo = new FileInfo(filePath);
        Console.WriteLine();
        ConsoleHelper.PrintSubHeader("File Information");
        Console.WriteLine($"  Name : {fileInfo.Name}");
        Console.WriteLine($"  Size : {FormatFileSize(fileInfo.Length)}");
        Console.WriteLine($"  Type : {fileInfo.Extension}");
        Console.WriteLine();

        string caseNumber = ConsoleHelper.GetInput("Case Number", required: false);
        string description = ConsoleHelper.GetInput("Description", required: false);

        Console.WriteLine();
        if (!ConsoleHelper.Confirm("Proceed with evidence upload?"))
        {
            ConsoleHelper.PrintWarning("Upload cancelled.");
            ConsoleHelper.Pause();
            return;
        }

        Console.WriteLine();

        var user = SessionManager.CurrentUser;
        if (user == null)
        {
            ConsoleHelper.PrintError("Session expired. Please log in again.");
            ConsoleHelper.Pause();
            return;
        }

        var request = new IngestionRequest(
            filePath,
            caseNumber,
            description,
            user.ID
        );

        // Show spinner while processing
        (bool success, string message, string? evidenceId) result = (false, "", null);

        ConsoleHelper.PrintSpinner("Processing evidence...", () =>
        {
            result = _evidenceService.IngestEvidence(request);
        });

        Console.WriteLine();
        if (result.success)
        {
            ConsoleHelper.PrintSuccess("Evidence uploaded successfully!");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  Evidence ID: {result.evidenceId,-52} ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
            ConsoleHelper.PrintInfo("Please save this Evidence ID for future verification.");
        }
        else
        {
            ConsoleHelper.PrintError(result.message);
        }

        ConsoleHelper.Pause();
    }

    public void ViewMyUploads()
    {
        Console.Clear();
        ConsoleHelper.PrintLogo();
        ConsoleHelper.PrintHeader("My Uploads");

        var user = SessionManager.CurrentUser;
        if (user == null)
        {
            ConsoleHelper.PrintError("Session expired.");
            ConsoleHelper.Pause();
            return;
        }

        var evidence = _evidenceService.GetMyEvidence(user.ID);

        if (evidence.Count == 0)
        {
            ConsoleHelper.PrintInfo("You have not uploaded any evidence yet.");
        }
        else
        {
            string[] headers = { "Evidence ID", "Original File", "Case #", "Date Uploaded" };
            var rows = evidence.Select(e => new[]
            {
                e.Ledger.ID,
                e.Metadata.OriginalFileName.Length > 25 
                    ? e.Metadata.OriginalFileName.Substring(0, 22) + "..." 
                    : e.Metadata.OriginalFileName,
                e.Metadata.CaseNumber ?? "N/A",
                new DateTime(e.Ledger.CreatedTicks).ToString("yyyy-MM-dd HH:mm")
            }).ToList();

            ConsoleHelper.PrintTable(headers, rows);

            Console.WriteLine();
            ConsoleHelper.PrintInfo($"Total: {evidence.Count} evidence file(s)");
        }

        ConsoleHelper.Pause();
    }

    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        int order = 0;
        double size = bytes;
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        return $"{size:0.##} {sizes[order]}";
    }
}
