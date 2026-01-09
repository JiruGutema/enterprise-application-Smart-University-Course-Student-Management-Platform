-- Initialize database schemas for Smart University
-- This script creates the necessary schemas for the modular architecture

-- Create schemas for each bounded context
CREATE SCHEMA IF NOT EXISTS identity;
CREATE SCHEMA IF NOT EXISTS courses;
CREATE SCHEMA IF NOT EXISTS enrollment;
CREATE SCHEMA IF NOT EXISTS assessments;
CREATE SCHEMA IF NOT EXISTS notifications;
CREATE SCHEMA IF NOT EXISTS ai;
CREATE SCHEMA IF NOT EXISTS content;
CREATE SCHEMA IF NOT EXISTS outbox;

-- Grant permissions to the postgres user
GRANT ALL PRIVILEGES ON SCHEMA identity TO postgres;
GRANT ALL PRIVILEGES ON SCHEMA courses TO postgres;
GRANT ALL PRIVILEGES ON SCHEMA enrollment TO postgres;
GRANT ALL PRIVILEGES ON SCHEMA assessments TO postgres;
GRANT ALL PRIVILEGES ON SCHEMA notifications TO postgres;
GRANT ALL PRIVILEGES ON SCHEMA ai TO postgres;
GRANT ALL PRIVILEGES ON SCHEMA content TO postgres;
GRANT ALL PRIVILEGES ON SCHEMA outbox TO postgres;

-- Create extensions if needed
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Log the initialization
SELECT 'Smart University database schemas initialized successfully' AS status;