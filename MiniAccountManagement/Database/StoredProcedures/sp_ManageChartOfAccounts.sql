CREATE PROCEDURE dbo.sp_ManageChartOfAccounts
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
        INSERT INTO dbo.ChartOfAccounts (AccountName, AccountCode, ParentAccountID)
        VALUES (@AccountName, @AccountCode, @ParentAccountID);
    END

    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE dbo.ChartOfAccounts
        SET 
            AccountName = @AccountName, 
            AccountCode = @AccountCode, 
            ParentAccountID = @ParentAccountID
        WHERE AccountID = @AccountID;
    END

    ELSE IF @Action = 'DELETE'
    BEGIN
        UPDATE dbo.ChartOfAccounts
        SET IsActive = 0
        WHERE AccountID = @AccountID;
    END

    ELSE IF @Action = 'SELECT_ALL'
    BEGIN
        SELECT AccountID, AccountName, AccountCode, ParentAccountID
        FROM dbo.ChartOfAccounts
        WHERE IsActive = 1
        ORDER BY AccountCode;
    END
     
    ELSE IF @Action = 'SELECT_BY_ID'
    BEGIN
        SELECT AccountID, AccountName, AccountCode, ParentAccountID
        FROM dbo.ChartOfAccounts
        WHERE AccountID = @AccountID AND IsActive = 1;
    END

END
GO