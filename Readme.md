# Digital Evidence Management System

### Design Thinking  or Architecture

#### 1. Since the application is going to statefull and be serving only one user per session we need to have a session manager which have certain roles like storing the information UserObject and permission if required( will be mentioned in the later design decisions)

> as per proposed plan i will add 3 roles to the application `admin`, `custodian`, `audiotor` this document will cover descrption and permission of these roles in detail in upcoming sections

#### 2. Ingestion Pipeline for Inputs 
we cannot save the files directly into storage after uploading the file the following logical steps must be follewed in order to make sure our file is stored properly

    1. Input path validation : validate if the given file path is valid.(making sure its not a shortcut or a 0byte files or a corrupted file).
    2. 