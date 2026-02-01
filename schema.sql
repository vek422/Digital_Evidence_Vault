-- Aether: Digital Evidence Management System
-- SQLite Schema

-- Users table (Identity)
CREATE TABLE IF NOT EXISTS User (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    username TEXT NOT NULL UNIQUE,
    password_hash TEXT NOT NULL,
    salt TEXT NOT NULL,
    role INTEGER NOT NULL,
    name TEXT NOT NULL
);

-- EvidenceLedger table (Immutable Vault - Write Once)
CREATE TABLE IF NOT EXISTS EvidenceLedger (
    id TEXT PRIMARY KEY,
    file_hash TEXT NOT NULL,
    integrity_signature TEXT NOT NULL,
    stored_file_name TEXT NOT NULL,
    uploader_id INTEGER NOT NULL,
    created_at_tick INTEGER NOT NULL,
    FOREIGN KEY (uploader_id) REFERENCES User(id)
);

-- EvidenceMetadata table (Mutable Context)
CREATE TABLE IF NOT EXISTS EvidenceMetadata (
    id TEXT PRIMARY KEY,
    original_file_name TEXT NOT NULL,
    description TEXT,
    file_extension TEXT NOT NULL,
    case_number TEXT,
    FOREIGN KEY (id) REFERENCES EvidenceLedger(id)
);

-- AuditLogs table (Surveillance)
CREATE TABLE IF NOT EXISTS AuditLogs (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    user_id INTEGER NOT NULL,
    action TEXT NOT NULL,
    timestamp TEXT NOT NULL,
    details TEXT,
    FOREIGN KEY (user_id) REFERENCES User(id)
);
