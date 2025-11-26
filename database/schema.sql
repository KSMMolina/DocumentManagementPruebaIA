-- Diseño normalizado para módulo de gestión documental (PostgreSQL)
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

DO $$ BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'user_role') THEN
        CREATE TYPE user_role AS ENUM ('Administrator', 'Resident', 'Porter');
    END IF;
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'audit_action') THEN
        CREATE TYPE audit_action AS ENUM (
            'FolderCreated', 'FolderRenamed', 'FolderDeleted',
            'FileUploaded', 'FileUpdated', 'FileDeleted', 'FileDownloaded',
            'PermissionGranted', 'PermissionRevoked'
        );
    END IF;
END $$;

CREATE TABLE IF NOT EXISTS properties (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(150) NOT NULL
);

CREATE TABLE IF NOT EXISTS users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    full_name VARCHAR(150) NOT NULL,
    email VARCHAR(200) NOT NULL UNIQUE,
    role user_role NOT NULL
);

CREATE TABLE IF NOT EXISTS property_users (
    property_id UUID NOT NULL REFERENCES properties(id) ON DELETE CASCADE,
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT pk_property_users PRIMARY KEY(property_id, user_id)
);

CREATE TABLE IF NOT EXISTS folders (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    property_id UUID NOT NULL REFERENCES properties(id) ON DELETE CASCADE,
    parent_folder_id UUID NULL REFERENCES folders(id) ON DELETE CASCADE,
    name VARCHAR(100) NOT NULL,
    depth SMALLINT NOT NULL CHECK (depth BETWEEN 1 AND 3),
    CONSTRAINT uq_folder_name UNIQUE(property_id, parent_folder_id, name)
);

CREATE INDEX IF NOT EXISTS idx_folder_search ON folders USING gin (to_tsvector('spanish', name));

CREATE TABLE IF NOT EXISTS files (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    folder_id UUID NOT NULL REFERENCES folders(id) ON DELETE CASCADE,
    name VARCHAR(150) NOT NULL,
    description VARCHAR(500) DEFAULT '',
    size_bytes BIGINT NOT NULL CHECK (size_bytes > 0 AND size_bytes <= 52428800),
    created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS idx_file_search ON files USING gin (to_tsvector('spanish', name));

CREATE TABLE IF NOT EXISTS permissions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    folder_id UUID NOT NULL REFERENCES folders(id) ON DELETE CASCADE,
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    can_view BOOLEAN NOT NULL DEFAULT FALSE,
    can_download BOOLEAN NOT NULL DEFAULT FALSE,
    CONSTRAINT chk_permission_access CHECK (can_view OR can_download),
    CONSTRAINT uq_permissions UNIQUE(folder_id, user_id)
);

CREATE TABLE IF NOT EXISTS audit_entries (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    property_id UUID NOT NULL REFERENCES properties(id) ON DELETE CASCADE,
    folder_id UUID NULL REFERENCES folders(id) ON DELETE SET NULL,
    file_id UUID NULL REFERENCES files(id) ON DELETE SET NULL,
    actor_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    action audit_action NOT NULL,
    occurred_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    details VARCHAR(500)
);

-- Restricción: máximo 2 subcarpetas por carpeta y profundidad máxima 3
CREATE OR REPLACE FUNCTION validate_folder_limits() RETURNS trigger AS $$
DECLARE
    current_depth SMALLINT;
    sibling_count INT;
BEGIN
    IF NEW.parent_folder_id IS NULL THEN
        current_depth := 1;
    ELSE
        SELECT depth INTO current_depth FROM folders WHERE id = NEW.parent_folder_id;
        IF current_depth IS NULL THEN
            RAISE EXCEPTION 'La carpeta padre no existe';
        END IF;
        IF current_depth + 1 > 3 THEN
            RAISE EXCEPTION 'No se permiten más de 3 niveles de carpetas';
        END IF;
        SELECT COUNT(*) INTO sibling_count FROM folders WHERE parent_folder_id = NEW.parent_folder_id;
        IF TG_OP = 'INSERT' THEN
            sibling_count := sibling_count + 1;
        END IF;
        IF sibling_count > 2 THEN
            RAISE EXCEPTION 'Cada carpeta solo puede tener 2 subcarpetas';
        END IF;
        NEW.depth := current_depth + 1;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER trg_validate_folder
BEFORE INSERT OR UPDATE ON folders
FOR EACH ROW EXECUTE FUNCTION validate_folder_limits();

-- Restricción: máximo 5 archivos por carpeta
CREATE OR REPLACE FUNCTION validate_file_limits() RETURNS trigger AS $$
DECLARE
    file_count INT;
BEGIN
    SELECT COUNT(*) INTO file_count FROM files WHERE folder_id = NEW.folder_id;
    IF TG_OP = 'INSERT' THEN
        file_count := file_count + 1;
    END IF;
    IF file_count > 5 THEN
        RAISE EXCEPTION 'La carpeta ya tiene 5 archivos';
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER trg_validate_file
BEFORE INSERT OR UPDATE ON files
FOR EACH ROW EXECUTE FUNCTION validate_file_limits();

-- Regla: el permiso del administrador no puede ser eliminado
CREATE OR REPLACE FUNCTION prevent_admin_permission_removal() RETURNS trigger AS $$
DECLARE
    user_role user_role;
BEGIN
    SELECT role INTO user_role FROM users WHERE id = OLD.user_id;
    IF user_role = 'Administrator' THEN
        RAISE EXCEPTION 'Los permisos del administrador no pueden eliminarse';
    END IF;
    RETURN OLD;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER trg_block_admin_permission_delete
BEFORE DELETE ON permissions
FOR EACH ROW EXECUTE FUNCTION prevent_admin_permission_removal();
