public class IngestionRequest
{
    public string FilePath { get; init; }
    public string CaseNumber { get; init; }
    public string Description { get; init; }
    public int UploaderId { get; init; }

    public IngestionRequest(string FilePath, string CaseNumber, string Description, int UploaderId)
    {
        this.FilePath = FilePath;
        this.CaseNumber = CaseNumber;
        this.Description = Description;
        this.UploaderId = UploaderId;
    }
}
