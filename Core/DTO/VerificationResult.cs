public class VerificationResult
{
    public bool IsAuthentic { get; init; }
    public bool IsDatabaseIntact { get; init; }
    public bool IsFileIntact { get; init; }
    public string Message { get; init; }
    public string EvidenceId { get; init; }

    public VerificationResult(bool IsAuthentic, bool IsDatabaseIntact, bool IsFileIntact, string Message, string EvidenceId)
    {
        this.IsAuthentic = IsAuthentic;
        this.IsDatabaseIntact = IsDatabaseIntact;
        this.IsFileIntact = IsFileIntact;
        this.Message = Message;
        this.EvidenceId = EvidenceId;
    }

    public static VerificationResult Success(string evidenceId)
    {
        return new VerificationResult(
            true,
            true,
            true,
            "Evidence is AUTHENTIC. No tampering detected.",
            evidenceId
        );
    }

    public static VerificationResult DatabaseTampered(string evidenceId)
    {
        return new VerificationResult(
            false,
            false,
            true,
            "CRITICAL: DATABASE TAMPERED! Integrity signature mismatch detected.",
            evidenceId
        );
    }

    public static VerificationResult FileCorrupted(string evidenceId)
    {
        return new VerificationResult(
            false,
            true,
            false,
            "CRITICAL: FILE CORRUPTED! File hash mismatch detected.",
            evidenceId
        );
    }

    public static VerificationResult NotFound(string evidenceId)
    {
        return new VerificationResult(
            false,
            false,
            false,
            "Evidence not found in the system.",
            evidenceId
        );
    }

    public static VerificationResult FileMissing(string evidenceId)
    {
        return new VerificationResult(
            false,
            true,
            false,
            "CRITICAL: FILE MISSING! Evidence file not found on disk.",
            evidenceId
        );
    }
}
