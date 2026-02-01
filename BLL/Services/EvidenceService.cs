using System.Configuration;

public class EvidenceService
{
    private readonly EvidenceRepository _evidenceRepo;
    private readonly AuditRepository _auditRepo;
    private readonly string _vaultPath;

    public EvidenceService()
    {
        _evidenceRepo = new EvidenceRepository();
        _auditRepo = new AuditRepository();
        _vaultPath = ConfigurationManager.AppSettings["VaultPath"] ?? "Vault";
    }

    public (bool Success, string Message, string? EvidenceId) IngestEvidence(IngestionRequest request)
    {
        if (!File.Exists(request.FilePath))
        {
            LogUploadFailure(request.UploaderId, "File not found: " + request.FilePath);
            return (false, "File not found.", null);
        }

        try
        {
            // Step 1: Generate unique ID
            string evidenceId = Guid.NewGuid().ToString();

            // Step 2: Compute file hash using streaming
            string fileHash = SecurityHelper.ComputeFileHash(request.FilePath);

            // Step 3: Get timestamp and generate integrity signature
            long createdTicks = DateTime.UtcNow.Ticks;
            string integritySignature = SecurityHelper.GenerateIntegritySignature(fileHash, createdTicks);

            // Step 4: Prepare vault storage path
            string year = DateTime.UtcNow.Year.ToString();
            string month = DateTime.UtcNow.Month.ToString("D2");
            string vaultDir = Path.Combine(_vaultPath, year, month);
            Directory.CreateDirectory(vaultDir);

            string originalFileName = Path.GetFileName(request.FilePath);
            string fileExtension = Path.GetExtension(request.FilePath);
            string storedFileName = $"{evidenceId}.dat";
            string storedFilePath = Path.Combine(vaultDir, storedFileName);
            string relativeStoredPath = Path.Combine(year, month, storedFileName);

            // Step 5: Copy file to vault
            File.Copy(request.FilePath, storedFilePath, overwrite: false);

            // Step 6: Create entities
            var ledger = new EvidenceLedger(
                evidenceId,
                fileHash,
                integritySignature,
                relativeStoredPath,
                request.UploaderId,
                createdTicks
            );

            var metadata = new EvidenceMetadata(
                request.CaseNumber,
                originalFileName,
                request.Description,
                fileExtension
            );

            // Step 7: Insert into database (transactional)
            try
            {
                _evidenceRepo.InsertEvidence(ledger, metadata);
            }
            catch
            {
                // Rollback: Delete the file from vault if DB insert fails
                if (File.Exists(storedFilePath))
                {
                    File.Delete(storedFilePath);
                }
                throw;
            }

            // Step 8: Log success
            _auditRepo.LogAction(
                request.UploaderId,
                LogAction.UploadSuccess,
                $"Evidence uploaded: {originalFileName} (ID: {evidenceId})"
            );

            return (true, $"Evidence ingested successfully. ID: {evidenceId}", evidenceId);
        }
        catch (Exception ex)
        {
            LogUploadFailure(request.UploaderId, ex.Message);
            return (false, $"Failed to ingest evidence: {ex.Message}", null);
        }
    }

    private void LogUploadFailure(int uploaderId, string details)
    {
        _auditRepo.LogAction(uploaderId, LogAction.UploadFail, details);
    }

    public List<(EvidenceLedger Ledger, EvidenceMetadata Metadata)> GetAllEvidence()
    {
        return _evidenceRepo.GetAllEvidence();
    }

    public List<(EvidenceLedger Ledger, EvidenceMetadata Metadata)> GetMyEvidence(int uploaderId)
    {
        return _evidenceRepo.GetEvidenceByUploader(uploaderId);
    }

    public int GetEvidenceCount()
    {
        return _evidenceRepo.GetEvidenceCount();
    }

    public string GetFullVaultPath(string relativeStoredPath)
    {
        return Path.Combine(_vaultPath, relativeStoredPath);
    }
}
