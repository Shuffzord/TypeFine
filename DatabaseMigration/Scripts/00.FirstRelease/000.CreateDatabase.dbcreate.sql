-- create database
PRINT N'Creating Database $DatabaseName$...'
GO

CREATE DATABASE [$DatabaseName$]
COLLATE Polish_CI_AS
GO

USE [$DatabaseName$]

SET DATEFIRST 1;
GO


