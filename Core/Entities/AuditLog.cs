public class AuditLog
{
    public long ID { get; init; }
    public int UserId { get; init; }
    public string Action { get; init; }
    public DateTime TimeStamp { get; init; }
    public string Details { get; init; }

    public AuditLog(long ID, int UserId, string Action, DateTime TimeStamp, string Details)
    {
        this.ID = ID;
        this.UserId = UserId;
        this.Action = Action;
        this.TimeStamp = TimeStamp;
        this.Details = Details;
    }
}