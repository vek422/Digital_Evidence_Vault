public class EvidenceLedger
{
    public string ID { get; init; }
    public string FileHash { get; set; }
    public string IntegritySignature { get; set; }
    public string StoredFileName { get; set; }
    public int UploaderID { get; set; }
    public long CreatedTicks { get; set; }

    public EvidenceLedger(string ID, string FileHash, string IntegritySignature, string StoredFileName, int UploaderID, long CreatedTicks)
    {
        this.ID = ID;
        this.FileHash = FileHash;
        this.IntegritySignature = IntegritySignature;
        this.StoredFileName = StoredFileName;
        this.UploaderID = UploaderID;
        this.CreatedTicks = CreatedTicks;
    }

}