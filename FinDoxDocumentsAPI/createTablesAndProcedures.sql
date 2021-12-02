--create schemas

DROP SCHEMA IF EXISTS users CASCADE;
CREATE SCHEMA users;

DROP SCHEMA IF EXISTS user_groups CASCADE;
CREATE SCHEMA user_groups;

DROP SCHEMA IF EXISTS documents CASCADE;
CREATE SCHEMA documents;

CREATE EXTENSION IF NOT EXISTS pgcrypto WITH SCHEMA users;
--create tables

DROP TABLE IF EXISTS users.users CASCADE;

CREATE TABLE users.users
(
	id SERIAL PRIMARY KEY,
	user_name TEXT NOT NULL UNIQUE,
	password TEXT NOT NULL,
	user_type INT DEFAULT 0 NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_users_user_name ON users.users (user_name) INCLUDE (password);

DROP TABLE IF EXISTS user_groups.user_groups CASCADE;

CREATE TABLE user_groups.user_groups
(
	id SERIAL PRIMARY KEY,
	user_group_name TEXT NOT NULL UNIQUE
);

DROP TABLE IF EXISTS user_groups.user_group_users CASCADE;

CREATE TABLE user_groups.user_group_users
(
	user_group_id int NOT NULL,
	user_id int NOT NULL,
	PRIMARY KEY (user_group_id, user_id),
	CONSTRAINT fk_user_group
		FOREIGN KEY (user_group_id)
			REFERENCES user_groups.user_groups(id)
			ON DELETE CASCADE,
	CONSTRAINT fk_user
		FOREIGN KEY (user_id)
			REFERENCES users.users(id)
			ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_user_group_users_user_group_id ON user_groups.user_group_users (user_group_id) INCLUDE (user_id);

DROP TABLE IF EXISTS documents.documents CASCADE;

CREATE TABLE documents.documents
(
	id SERIAL PRIMARY KEY,
	upload_timestamp TIMESTAMP DEFAULT NOW() NOT NULL,
	document_name TEXT NOT NULL UNIQUE,
	description TEXT NULL,
	category INT NOT NULL
);

DROP TABLE IF EXISTS documents.document_users CASCADE;

CREATE TABLE documents.document_users
(
	document_id int NOT NULL,
	user_id int NOT NULL,
	PRIMARY KEY (document_id, user_id),
	CONSTRAINT fk_document
		FOREIGN KEY (document_id)
			REFERENCES documents.documents(id)
			ON DELETE CASCADE,
	CONSTRAINT fk_user
		FOREIGN KEY (user_id)
			REFERENCES users.users(id)
			ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_document_users_document_id ON documents.document_users (document_id) INCLUDE (user_id);

DROP TABLE IF EXISTS documents.document_user_groups CASCADE;

CREATE TABLE documents.document_user_groups
(
	document_id int NOT NULL,
	user_group_id int NOT NULL,
	PRIMARY KEY (document_id, user_group_id),
	CONSTRAINT fk_document
		FOREIGN KEY (document_id)
			REFERENCES documents.documents(id)
			ON DELETE CASCADE,
	CONSTRAINT fk_user_groups
		FOREIGN KEY (user_group_id)
			REFERENCES user_groups.user_groups(id)
			ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_document_user_groups_document_id ON documents.document_user_groups (document_id) INCLUDE (user_group_id);

INSERT INTO users.users(user_name, password, user_type)
VALUES ('admin', crypt('1', gen_salt('bf')), 2);

--create functions
--users

CREATE OR REPLACE FUNCTION users.get_user(id INT DEFAULT NULL) RETURNS TABLE (user_id INT, user_name TEXT, user_type INT)
LANGUAGE sql
AS
$$
	SELECT id AS user_id, user_name, user_type FROM users.users WHERE (get_user.id IS NULL OR get_user.id = id)
$$;

CREATE OR REPLACE FUNCTION users.get_user_from_credentials(user_name TEXT, password TEXT) RETURNS TABLE (user_id INT, user_name TEXT, user_type INT)
LANGUAGE sql
AS
$$
	SELECT id AS user_id, user_name, user_type FROM users.users WHERE (user_name = get_user_from_credentials.user_name AND password = crypt(get_user_from_credentials.password, password))
$$;

CREATE OR REPLACE FUNCTION users.new_user(user_name text, password text, user_type int) RETURNS TABLE (user_id INT, user_name TEXT, user_type INT)
LANGUAGE sql
AS
$$
	INSERT INTO users.users(user_name, password, user_type)
	VALUES (user_name, crypt(password, gen_salt('bf')), user_type)
	RETURNING id AS user_id, user_name, user_type
$$;

CREATE OR REPLACE FUNCTION users.update_user(id int, user_name text, password text, user_type int) RETURNS TABLE (user_id INT, user_name TEXT, user_type INT)
LANGUAGE sql
AS
$$
	UPDATE users.users
	SET user_name = update_user.user_name,
		password = crypt(update_user.password, gen_salt('bf')),
		user_type = update_user.user_type
	WHERE id = update_user.id
	RETURNING id AS user_id, user_name, user_type
$$;

CREATE OR REPLACE FUNCTION users.delete_user(id int) RETURNS VOID
LANGUAGE sql
AS
$$
	DELETE FROM users.users WHERE id = delete_user.id
$$;

--user_groups

CREATE OR REPLACE FUNCTION user_groups.get_user_group(id INT DEFAULT NULL) RETURNS TABLE (user_group_id INT, user_group_name TEXT, user_id INT, user_name TEXT, user_type INT)
LANGUAGE sql
AS
$$
	SELECT ug.id AS user_group_id, ug.user_group_name, u.id AS user_id, u.user_name, u.user_type
	FROM user_groups.user_groups ug
	LEFT JOIN user_groups.user_group_users ugu ON ugu.user_group_id = ug.id
	JOIN users.users u ON u.id = ugu.user_id
	WHERE (get_user_group.id IS NULL OR get_user_group.id = ug.id)
$$;

CREATE OR REPLACE FUNCTION user_groups.new_user_group(_user_group_name TEXT, members INT[]) RETURNS TABLE (user_group_id INT, user_group_name TEXT, user_id INT, user_name TEXT, user_type INT)
LANGUAGE plpgsql
AS
$$
	DECLARE 
		inserted_id INT;
	BEGIN
		INSERT INTO user_groups.user_groups (user_group_name)
		VALUES (new_user_group._user_group_name)
		RETURNING id INTO inserted_id;
		
		INSERT INTO user_groups.user_group_users (user_group_id, user_id)
		SELECT inserted_id, * FROM unnest(members);
		
		RETURN QUERY SELECT * FROM user_groups.get_user_group(inserted_id);
	END;
$$;

CREATE OR REPLACE FUNCTION user_groups.update_user_group(id INT, _user_group_name TEXT, members INT[]) RETURNS TABLE (user_group_id INT, user_group_name TEXT, user_id INT, user_name TEXT, user_type INT)
LANGUAGE plpgsql
AS
$$
	DECLARE 
		updated_id INT;
	BEGIN
		UPDATE user_groups.user_groups u
		SET user_group_name = update_user_group._user_group_name
		WHERE u.id = update_user_group.id
		RETURNING u.id INTO updated_id;
		
		DELETE FROM user_groups.user_group_users ugu
		WHERE ugu.user_group_id = update_user_group.id;
		
		INSERT INTO user_groups.user_group_users (user_group_id, user_id)
		SELECT updated_id, * FROM unnest(members);
		
		RETURN QUERY SELECT * FROM user_groups.get_user_group(updated_id);
	END;
$$;

CREATE OR REPLACE FUNCTION user_groups.delete_user_group(id int) RETURNS VOID
LANGUAGE sql
AS
$$
	DELETE FROM user_groups.user_group_users WHERE user_group_id = delete_user_group.id;
	
	DELETE FROM user_groups.user_groups WHERE id = delete_user_group.id;
$$;

--documents

CREATE OR REPLACE FUNCTION documents.get_document(id INT, user_id INT, check_user BOOLEAN) RETURNS TABLE (document_id INT, upload_timestamp TIMESTAMP, document_name TEXT, description TEXT, category INT)
LANGUAGE plpgsql
AS
$$
	BEGIN
		IF (NOT EXISTS(SELECT 1 FROM documents.documents d WHERE d.id = get_document.id)) THEN
			RAISE EXCEPTION 'document does not exist';
		END IF;
	
		IF (check_user AND NOT EXISTS(SELECT 1 FROM documents.document_users du WHERE du.document_id = get_document.id AND du.user_id = get_document.user_id)
			AND NOT EXISTS(SELECT 1 FROM documents.document_user_groups dug
						   JOIN user_groups.user_group_users ugu ON ugu.user_group_id = dug.user_group_id
						   WHERE dug.document_id = get_document.id AND ugu.user_id = get_document.user_id)) THEN
			RAISE EXCEPTION 'user does not have access to this document';
		END IF;

		RETURN QUERY SELECT d.id AS document_id, d.upload_timestamp, d.document_name, d.description, d.category
		FROM documents.documents d
		WHERE (get_document.id = d.id);
	END;
$$;

CREATE OR REPLACE FUNCTION documents.get_documents_by_user(user_id INT DEFAULT NULL) RETURNS TABLE (document_id INT, upload_timestamp TIMESTAMP, document_name TEXT, description TEXT, category INT)
LANGUAGE plpgsql
AS
$$
	BEGIN
		RETURN QUERY SELECT DISTINCT d.id AS document_id, d.upload_timestamp, d.document_name, d.description, d.category
		FROM documents.documents d
		LEFT JOIN documents.document_users du ON du.document_id = d.id
		LEFT JOIN documents.document_user_groups dug ON dug.document_id = d.id
		LEFT JOIN user_groups.user_group_users ugu ON ugu.user_group_id = dug.user_group_id
		WHERE (get_documents_by_user.user_id IS NULL OR (du.user_id = get_documents_by_user.user_id OR ugu.user_id = get_documents_by_user.user_id));
	END;
$$;

CREATE OR REPLACE FUNCTION documents.search_documents(_document_name TEXT, _description TEXT, _category INT, user_id INT, check_user BOOLEAN) RETURNS TABLE (document_id INT, upload_timestamp TIMESTAMP, document_name TEXT, description TEXT, category INT)
LANGUAGE plpgsql
AS
$$
	DECLARE
		search_user INT DEFAULT NULL;
	BEGIN
		CREATE TEMP TABLE IF NOT EXISTS user_documents (document_id INT, upload_timestamp TIMESTAMP, document_name TEXT, description TEXT, category INT);
		
		IF (NOT check_user) THEN
			search_user := search_documents.user_id;
		END IF;
		
		INSERT INTO user_documents (document_id, upload_timestamp, document_name, description, category)
		SELECT * FROM documents.get_documents_by_user(search_user);

		RETURN QUERY SELECT ud.document_id, ud.upload_timestamp, ud.document_name, ud.description, ud.category
		FROM user_documents ud
		WHERE ((search_documents._document_name IS NULL OR ud.document_name = search_documents._document_name)
			  AND (search_documents._description IS NULL OR ud.description = search_documents._description)
			  AND (search_documents._category IS NULL OR ud.category = search_documents._category));
		
		DROP TABLE user_documents;
	END;
$$;

CREATE OR REPLACE FUNCTION documents.new_document(_document_name TEXT, _description TEXT, _category INT, users INT[], user_groups INT[]) RETURNS TABLE (document_id INT, upload_timestamp TIMESTAMP, document_name TEXT, description TEXT, category INT)
LANGUAGE plpgsql
AS
$$
	DECLARE 
		inserted_id INT;
	BEGIN
		INSERT INTO documents.documents (document_name, description, category)
		VALUES (new_document._document_name, new_document._description, new_document._category)
		RETURNING id INTO inserted_id;
		
		INSERT INTO documents.document_users (document_id, user_id)
		SELECT inserted_id, * FROM unnest(users);
		
		INSERT INTO documents.document_user_groups (document_id, user_group_id)
		SELECT inserted_id, * FROM unnest(user_groups);

		RETURN QUERY SELECT d.id as document_id, d.upload_timestamp, d.document_name, d.description, d.category
		FROM documents.documents d
		WHERE (inserted_id = d.id);
	END;
$$;

CREATE OR REPLACE FUNCTION documents.update_document(id INT, _document_name TEXT, _description TEXT, _category INT, users INT[], user_groups INT[], user_id INT, check_user BOOLEAN) RETURNS TABLE (document_id INT, upload_timestamp TIMESTAMP, document_name TEXT, description TEXT, category INT)
LANGUAGE plpgsql
AS
$$
	DECLARE 
		updated_id INT;
	BEGIN
		IF (NOT EXISTS(SELECT 1 FROM documents.documents d WHERE d.id = update_document.id)) THEN
			RAISE EXCEPTION 'document does not exist';
		END IF;
	
		IF (check_user AND NOT EXISTS(SELECT 1 FROM documents.document_users du WHERE du.document_id = update_document.id AND du.user_id = update_document.user_id)
			AND NOT EXISTS(SELECT 1 FROM documents.document_user_groups dug
						   JOIN user_groups.user_group_users ugu ON ugu.user_group_id = dug.user_group_id
						   WHERE dug.document_id = update_document.id AND ugu.user_id = update_document.user_id)) THEN
			RAISE EXCEPTION 'user does not have access to this document';
		END IF;
		
		UPDATE documents.documents d
		SET document_name = update_document._document_name,
		description = update_document._description,
		category = update_document._category
		WHERE d.id = update_document.id
		RETURNING d.id INTO updated_id;
		
		DELETE FROM documents.document_users du WHERE du.document_id = update_document.id;
		
		DELETE FROM documents.document_user_groups dug WHERE dug.document_id = update_document.id;	
		
		INSERT INTO documents.document_users (document_id, user_id)
		SELECT updated_id, * FROM unnest(users);
		
		INSERT INTO documents.document_user_groups (document_id, user_group_id)
		SELECT updated_id, * FROM unnest(user_groups);

		RETURN QUERY SELECT d.id as document_id, d.upload_timestamp, d.document_name, d.description, d.category
		FROM documents.documents d
		WHERE (updated_id = d.id);
	END;
$$;

CREATE OR REPLACE FUNCTION documents.delete_document(id int, user_id INT, check_user BOOLEAN) RETURNS VOID
LANGUAGE plpgsql
AS
$$
	BEGIN
		IF (NOT EXISTS(SELECT 1 FROM documents.documents d WHERE d.id = delete_document.id)) THEN
			RAISE EXCEPTION 'document does not exist';
		END IF;

		IF (check_user AND NOT EXISTS(SELECT 1 FROM documents.document_users du WHERE du.document_id = delete_document.id AND du.user_id = delete_document.user_id)
			AND NOT EXISTS(SELECT 1 FROM documents.document_user_groups dug
						   JOIN user_groups.user_group_users ugu ON ugu.user_group_id = dug.user_group_id
						   WHERE dug.document_id = delete_document.id AND ugu.user_id = delete_document.user_id)) THEN
			RAISE EXCEPTION 'user does not have access to this document';
		END IF;

		DELETE FROM documents.document_users du WHERE du.document_id = delete_document.id;

		DELETE FROM documents.document_user_groups dug WHERE dug.document_id = delete_document.id;	

		DELETE FROM documents.documents d WHERE d.id = delete_document.id;
	END;
$$;

