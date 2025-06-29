CREATE TABLE dbo.ChartOfAccounts (
    AccountID INT PRIMARY KEY IDENTITY(1,1),

    AccountName NVARCHAR(255) NOT NULL, 

    AccountCode VARCHAR(50) UNIQUE NOT NULL,

    ParentAccountID INT NULL,
    IsActive BIT NOT NULL DEFAULT 1, 

    FOREIGN KEY (ParentAccountID) REFERENCES dbo.ChartOfAccounts(AccountID)
);
GO