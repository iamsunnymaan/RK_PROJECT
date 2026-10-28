CREATE DATABASE CustomerPortalMvcDb;
GO

USE CustomerPortalMvcDb;
GO

CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    FullName NVARCHAR(100) NOT NULL,
    Password NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE Customers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    Phone NVARCHAR(20) NOT NULL,
    Address NVARCHAR(250) NULL,
    City NVARCHAR(100) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedOn DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedOn DATETIME2 NULL
);
GO

INSERT INTO Users (Username, FullName, Password)
VALUES ('admin', 'Administrator', 'Admin@123');
GO


--------check the tables in the databases-------
select * from sys.databases;
use CustomerPortalMvcDb;
select * from sys.tables;


select * from Users;
select * from Customers;