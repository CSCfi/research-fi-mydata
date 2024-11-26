-- Create database
IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'keycloak')
    CREATE DATABASE keycloak;
GO