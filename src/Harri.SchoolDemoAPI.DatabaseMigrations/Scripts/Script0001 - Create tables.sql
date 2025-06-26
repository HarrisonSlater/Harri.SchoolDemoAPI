CREATE SCHEMA [SchoolDemo]
GO

CREATE TABLE [SchoolDemo].Student (
    [sID] int IDENTITY PRIMARY KEY NOT NULL,
    [sName] varchar(255) NOT NULL,
    [GPA] decimal(5,2) NULL
);

CREATE TABLE [SchoolDemo].School (
    [schoolID] int IDENTITY(1001, 1) PRIMARY KEY NOT NULL,
    [schoolName] varchar(255) NOT NULL,
    [state] varchar(3) NOT NULL,
    [enrollment] int NULL
);

CREATE TABLE [SchoolDemo].[Application] (
    [applicationID] int NOT NULL IDENTITY PRIMARY KEY,
    [sID] int NOT NULL FOREIGN KEY REFERENCES [SchoolDemo].Student,
    [schoolID] int NOT NULL FOREIGN KEY REFERENCES [SchoolDemo].School,
    [major] VARCHAR(255) NULL,
    [decision] char(1) NOT NULL
);

