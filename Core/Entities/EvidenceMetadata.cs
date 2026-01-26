public class EvidenceMetadata
{
    public string CaseNumber { get; set; }
    public string OriginalFileName { get; set; }
    public string Description { get; set; }
    public string FileExtension { get; set; }

    public EvidenceMetadata(string CaseNumber, string OriginalFileName, string Description, string FileExtension)
    {
        this.CaseNumber = CaseNumber;
        this.OriginalFileName = OriginalFileName;
        this.Description = Description;
        this.FileExtension = FileExtension;
    }
}