using System.Configuration;

public class VerificationService
{
    private readonly EvidenceRepository _evidenceRepo;
    private readonly AuditRepository _auditRepo;
    private readonly string _vaultPath;

    public VerificationService()
    {
        _evidenceRepo = new EvidenceRepository();
        _auditRepo = new AuditRepository();
        _vaultPath = ConfigurationManager.AppSettings["VaultPath"] ?? "Vault";
    }

    public VerificationResult VerifyEvidence(string evidenceId)
    {
        int userId = SessionManager.CurrentUser?.ID ?? 0;

        // Step 1: Retrieve ledger from database
        var ledger = _evidenceRepo.GetLedgerById(evidenceId);
        if (ledger == null)
        {
            LogVerification(userId, evidenceId, false, "Evidence not found");
            return VerificationResult.NotFound(evidenceId);
        }

        // Step 2: Verify database integrity (HMAC check)
        // Recalculate HMAC from stored data and compare with stored signature
        bool isDatabaseIntact = SecurityHelper.VerifyIntegritySignature(
            ledger.FileHash,
            ledger.CreatedTicks,
            ledger.IntegritySignature
        );

        if (!isDatabaseIntact)
        {
            LogVerification(userId, evidenceId, false, "DATABASE TAMPERED - Signature mismatch");
            return VerificationResult.DatabaseTampered(evidenceId);
        }

        // Step 3: Verify file integrity (Hash check)
        string filePath = Path.Combine(_vaultPath, ledger.StoredFileName);

        if (!File.Exists(filePath))
        {
            LogVerification(userId, evidenceId, false, "FILE MISSING from vault");
            return VerificationResult.FileMissing(evidenceId);
        }

        string currentFileHash = SecurityHelper.ComputeFileHash(filePath);
        bool isFileIntact = string.Equals(currentFileHash, ledger.FileHash, StringComparison.OrdinalIgnoreCase);

        if (!isFileIntact)
        {
            LogVerification(userId, evidenceId, false, "FILE CORRUPTED - Hash mismatch");
            return VerificationResult.FileCorrupted(evidenceId);
        }

        // Step 4: Both checks passed - Evidence is authentic
        LogVerification(userId, evidenceId, true, "Evidence verified as AUTHENTIC");
        return VerificationResult.Success(evidenceId);
    }

    private void LogVerification(int userId, string evidenceId, bool success, string details)
    {
        _auditRepo.LogAction(
            userId,
            success ? LogAction.VerifySuccess : LogAction.VerifyFail,
            $"Evidence ID: {evidenceId} - {details}"
        );
    }

    public (EvidenceLedger? Ledger, EvidenceMetadata? Metadata) GetEvidenceDetails(string evidenceId)
    {
        var ledger = _evidenceRepo.GetLedgerById(evidenceId);
        var metadata = _evidenceRepo.GetMetadataById(evidenceId);
        return (ledger, metadata);
    }

    public List<(EvidenceLedger Ledger, EvidenceMetadata Metadata)> GetAllEvidence()
    {
        return _evidenceRepo.GetAllEvidence();
    }
}
