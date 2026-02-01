using System.Data;
using Microsoft.Data.Sqlite;

public class EvidenceRepository
{
    private EvidenceLedger MapRowToLedger(DataRow data)
    {
        return new EvidenceLedger(
            data["id"].ToString()!,
            data["file_hash"].ToString()!,
            data["integrity_signature"].ToString()!,
            data["stored_file_name"].ToString()!,
            Convert.ToInt32(data["uploader_id"]),
            Convert.ToInt64(data["created_at_tick"])
        );
    }

    private EvidenceMetadata MapRowToMetadata(DataRow data)
    {
        return new EvidenceMetadata(
            data["case_number"]?.ToString() ?? "",
            data["original_file_name"].ToString()!,
            data["description"]?.ToString() ?? "",
            data["file_extension"].ToString()!
        );
    }

    public bool InsertEvidence(EvidenceLedger ledger, EvidenceMetadata metadata)
    {
        using var connection = DatabaseHelper.GetConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            // Insert into EvidenceLedger
            string ledgerSql = @"
                INSERT INTO EvidenceLedger (id, file_hash, integrity_signature, stored_file_name, uploader_id, created_at_tick)
                VALUES (@id, @hash, @sig, @stored, @uploader, @ticks)";

            using (var cmd = new SqliteCommand(ledgerSql, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@id", ledger.ID);
                cmd.Parameters.AddWithValue("@hash", ledger.FileHash);
                cmd.Parameters.AddWithValue("@sig", ledger.IntegritySignature);
                cmd.Parameters.AddWithValue("@stored", ledger.StoredFileName);
                cmd.Parameters.AddWithValue("@uploader", ledger.UploaderID);
                cmd.Parameters.AddWithValue("@ticks", ledger.CreatedTicks);
                cmd.ExecuteNonQuery();
            }

            // Insert into EvidenceMetadata
            string metaSql = @"
                INSERT INTO EvidenceMetadata (id, original_file_name, description, file_extension, case_number)
                VALUES (@id, @original, @desc, @ext, @case)";

            using (var cmd = new SqliteCommand(metaSql, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@id", ledger.ID);
                cmd.Parameters.AddWithValue("@original", metadata.OriginalFileName);
                cmd.Parameters.AddWithValue("@desc", metadata.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ext", metadata.FileExtension);
                cmd.Parameters.AddWithValue("@case", metadata.CaseNumber ?? (object)DBNull.Value);
                cmd.ExecuteNonQuery();
            }

            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public EvidenceLedger? GetLedgerById(string id)
    {
        string sql = "SELECT * FROM EvidenceLedger WHERE id = @id";
        var parameters = new[] { new SqliteParameter("@id", id) };

        DataTable table = DatabaseHelper.ExecuteQuery(sql, parameters);
        if (table.Rows.Count == 0) return null;

        return MapRowToLedger(table.Rows[0]);
    }

    public EvidenceMetadata? GetMetadataById(string id)
    {
        string sql = "SELECT * FROM EvidenceMetadata WHERE id = @id";
        var parameters = new[] { new SqliteParameter("@id", id) };

        DataTable table = DatabaseHelper.ExecuteQuery(sql, parameters);
        if (table.Rows.Count == 0) return null;

        return MapRowToMetadata(table.Rows[0]);
    }

    public List<(EvidenceLedger Ledger, EvidenceMetadata Metadata)> GetAllEvidence()
    {
        string sql = @"
            SELECT l.*, m.original_file_name, m.description, m.file_extension, m.case_number
            FROM EvidenceLedger l
            INNER JOIN EvidenceMetadata m ON l.id = m.id
            ORDER BY l.created_at_tick DESC";

        DataTable table = DatabaseHelper.ExecuteQuery(sql);
        var results = new List<(EvidenceLedger, EvidenceMetadata)>();

        foreach (DataRow row in table.Rows)
        {
            var ledger = MapRowToLedger(row);
            var metadata = MapRowToMetadata(row);
            results.Add((ledger, metadata));
        }

        return results;
    }

    public List<(EvidenceLedger Ledger, EvidenceMetadata Metadata)> GetEvidenceByUploader(int uploaderId)
    {
        string sql = @"
            SELECT l.*, m.original_file_name, m.description, m.file_extension, m.case_number
            FROM EvidenceLedger l
            INNER JOIN EvidenceMetadata m ON l.id = m.id
            WHERE l.uploader_id = @uploader
            ORDER BY l.created_at_tick DESC";

        var parameters = new[] { new SqliteParameter("@uploader", uploaderId) };
        DataTable table = DatabaseHelper.ExecuteQuery(sql, parameters);
        var results = new List<(EvidenceLedger, EvidenceMetadata)>();

        foreach (DataRow row in table.Rows)
        {
            var ledger = MapRowToLedger(row);
            var metadata = MapRowToMetadata(row);
            results.Add((ledger, metadata));
        }

        return results;
    }

    public int GetEvidenceCount()
    {
        string sql = "SELECT COUNT(*) FROM EvidenceLedger";
        var result = DatabaseHelper.ExecuteScalar(sql);
        return Convert.ToInt32(result);
    }
}