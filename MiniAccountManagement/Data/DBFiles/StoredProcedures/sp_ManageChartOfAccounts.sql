USE [MiniAccountManagementDB]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER   PROCEDURE [dbo].[sp_ManageChartOfAccounts]
    @Action NVARCHAR(20),
    @AccountID INT = NULL,
    @AccountName NVARCHAR(255) = NULL,
    @AccountCode VARCHAR(50) = NULL,
    @ParentAccountID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Action = 'CREATE'
    BEGIN
        DECLARE @NewAccountCode VARCHAR(50);

        IF @ParentAccountID IS NULL
        BEGIN
            DECLARE @MaxTopLevelCode BIGINT;
            SELECT @MaxTopLevelCode = ISNULL(MAX(CAST(AccountCode AS BIGINT)), 1000000) 
            FROM dbo.ChartOfAccounts WHERE ParentAccountID IS NULL;
            SET @NewAccountCode = CAST((@MaxTopLevelCode + 1) AS VARCHAR(50));
        END
        ELSE
        BEGIN
            DECLARE @MaxChildCode VARCHAR(50);
            SELECT @MaxChildCode = MAX(AccountCode) FROM dbo.ChartOfAccounts WHERE ParentAccountID = @ParentAccountID;

            IF @MaxChildCode IS NULL
            BEGIN
                DECLARE @ParentCode VARCHAR(50);
                SELECT @ParentCode = AccountCode FROM dbo.ChartOfAccounts WHERE AccountID = @ParentAccountID;
                SET @NewAccountCode = @ParentCode + '01';
            END
            ELSE
            BEGIN
                SET @NewAccountCode = CAST((CAST(@MaxChildCode AS BIGINT) + 1) AS VARCHAR(50));
            END
        END

        INSERT INTO dbo.ChartOfAccounts (AccountName, AccountCode, ParentAccountID)
        VALUES (@AccountName, @NewAccountCode, @ParentAccountID);
    END
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE dbo.ChartOfAccounts SET AccountName = @AccountName, AccountCode = @AccountCode, ParentAccountID = @ParentAccountID WHERE AccountID = @AccountID;
    END
    ELSE IF @Action = 'DELETE'
    BEGIN
        UPDATE dbo.ChartOfAccounts SET IsActive = 0 WHERE AccountID = @AccountID;
    END
    ELSE IF @Action = 'SELECT_ALL'
    BEGIN
        SELECT AccountID, AccountName, AccountCode, ParentAccountID FROM dbo.ChartOfAccounts WHERE IsActive = 1 ORDER BY AccountCode;
    END
    ELSE IF @Action = 'SELECT_BY_ID'
    BEGIN
        SELECT AccountID, AccountName, AccountCode, ParentAccountID FROM dbo.ChartOfAccounts WHERE AccountID = @AccountID AND IsActive = 1;
    END
END
