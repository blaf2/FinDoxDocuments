--create schemas

DROP SCHEMA IF EXISTS documents CASCADE;
CREATE SCHEMA documents;

--create tables

DROP TABLE IF EXISTS documents.documents CASCADE;

CREATE TABLE documents.documents
(
	id SERIAL PRIMARY KEY,
	metadata_id INT NOT NULL UNIQUE,
	content BYTEA NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_documents_metadata_id ON documents.documents (metadata_id);

--create functions

CREATE OR REPLACE FUNCTION documents.get_document(metadata_id INT DEFAULT NULL) RETURNS TABLE (id INT, metadata_id INT, content BYTEA)
LANGUAGE sql
AS
$$
	SELECT id, metadata_id, content FROM documents.documents WHERE (get_document.metadata_id IS NULL OR get_document.metadata_id = metadata_id)
$$;

CREATE OR REPLACE FUNCTION documents.new_document(metadata_id INT, content BYTEA) RETURNS TABLE (id INT, metadata_id INT, content BYTEA)
LANGUAGE sql
AS
$$
	INSERT INTO documents.documents (metadata_id, content)
	VALUES (new_document.metadata_id, new_document.content)
	RETURNING id, metadata_id, content;
$$;

CREATE OR REPLACE FUNCTION documents.delete_document(metadata_id INT) RETURNS VOID
LANGUAGE sql
AS
$$
	DELETE FROM documents.documents WHERE metadata_id = delete_document.metadata_id
$$;
