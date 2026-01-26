public class AuditLog
{
    public long ID { get; init; }
    public string EvidenceId { get; init; }
    public string Action { get; set; }
    public DateTime TimeStamp { get; init; }
    public string Details { get; set; }


    public AuditLog(long ID, string EvidenceId, string Action, DateTime TimeStamp, string Details)
    {
        this.ID = ID;
        this.EvidenceId = EvidenceId;
        this.Action = Action;
        this.TimeStamp = TimeStamp;
        this.Details = Details;
    }
}