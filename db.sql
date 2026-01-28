CREATE TABLE User(
    id INT PRIMARY KEY AUTO_INCREMENT,
    username VARCHAR(50) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    salt VARCHAR(64) NOT NULL,
    role INT NOT NULL,
    name VARCHAR(100) NOT NULL
);

CREATE TABLE EvidenceLedger(
    id Binary(16) PRIMARY KEY,
    filehash CHAR(64) NOT NULL,
    integrity_signature CHAR(64) NOT NULL,
    stored_file_name VARCHAR(100) NOT NULL,
    uploader_id INT NOT NULL,
    created_at_tick BIGINT NOT NULL,
    Foreign Key (uploader_id) References User(id)
);

CREATE TABLE EvidenceMetadata(
    id binary(16) PRIMARY KEY,
    original_file_name varchar(255) NOT NULL,
    description TEXT ,
    file_extension VARCHAR(10) NOT NULL,
    case_number VARCHAR(50),
    Foreign Key (id) References EvidenceLedger(id)
);

CREATE TABLE AuditLogs(
    id INT PRIMARY KEY AUTO_INCREMENT,
    user_id INT NOT NULL,
    action VARCHAR(255) NOT NULL,
    timestamp DATETIME NOT NULL
    details TEXT,
    Foreign Key (user_id) References User(id)
);