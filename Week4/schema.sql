-- Database creation schema for SQL Server

CREATE TABLE Users (
    Username NVARCHAR(50) NOT NULL PRIMARY KEY,
    PasswordHash VARBINARY(64) NOT NULL,
    PasswordSalt VARBINARY(64) NOT NULL,
    Role NVARCHAR(20) NOT NULL
);

CREATE TABLE Students (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Age INT NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Grade DECIMAL(5,2) NOT NULL,
    InternalNotes NVARCHAR(MAX) NULL,
    EnrolledOn DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

CREATE TABLE Teachers (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Subject NVARCHAR(100) NOT NULL,
    Salary DECIMAL(18,2) NOT NULL,
    InternalNotes NVARCHAR(MAX) NULL
);

GO

-- Stored Procedures for Day 2 Task 4.4
CREATE PROCEDURE usp_GetStudentById
    @Id INT
AS
BEGIN
    SELECT Id, Name, Age, Email, Grade, InternalNotes, EnrolledOn
    FROM Students
    WHERE Id = @Id;
END;

GO

CREATE PROCEDURE usp_InsertStudent
    @Name NVARCHAR(100),
    @Age INT,
    @Email NVARCHAR(100),
    @Grade DECIMAL(5,2),
    @InternalNotes NVARCHAR(MAX)
AS
BEGIN
    INSERT INTO Students (Name, Age, Email, Grade, InternalNotes, EnrolledOn)
    VALUES (@Name, @Age, @Email, @Grade, @InternalNotes, GETUTCDATE());
    
    SELECT SCOPE_IDENTITY() AS NewId;
END;

GO
