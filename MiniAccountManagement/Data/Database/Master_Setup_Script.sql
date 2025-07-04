CREATE TABLE dbo.ChartOfAccounts
(
    AccountID INT PRIMARY KEY IDENTITY(1,1),
    AccountName NVARCHAR(255) NOT NULL,
    AccountCode VARCHAR(50) UNIQUE NOT NULL,
    ParentAccountID INT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (ParentAccountID) REFERENCES dbo.ChartOfAccounts(AccountID)
);
GO

CREATE TABLE dbo.VoucherMaster
(
    VoucherMasterID BIGINT PRIMARY KEY IDENTITY(1,1),
    VoucherDate DATE NOT NULL,
    VoucherType NVARCHAR(50) NOT NULL,
    ReferenceNo NVARCHAR(100) NULL,
    Narration NVARCHAR(500) NULL,
    CreatedBy NVARCHAR(450) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id)
);
GO

CREATE TABLE dbo.VoucherDetails
(
    VoucherDetailID BIGINT PRIMARY KEY IDENTITY(1,1),
    VoucherMasterID BIGINT NOT NULL,
    AccountID INT NOT NULL,
    DebitAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    CreditAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,

    FOREIGN KEY (VoucherMasterID) REFERENCES dbo.VoucherMaster(VoucherMasterID) ON DELETE CASCADE,
    FOREIGN KEY (AccountID) REFERENCES dbo.ChartOfAccounts(AccountID)
);
GO

IF TYPE_ID(N'dbo.VoucherDetailType') IS NOT NULL
    DROP TYPE dbo.VoucherDetailType;
GO

CREATE TYPE dbo.VoucherDetailType AS TABLE(
    AccountID INT NOT NULL,
    DebitAmount DECIMAL(18, 2) NOT NULL,
    CreditAmount DECIMAL(18, 2) NOT NULL
);
GO



CREATE OR ALTER PROCEDURE dbo.sp_CheckUserHasVouchers
    @UserId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST(
        CASE WHEN EXISTS (SELECT 1 FROM dbo.VoucherMaster WHERE CreatedBy = @UserId)
        THEN 1
        ELSE 0
    END AS BIT);
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetDashboardStats
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        (SELECT COUNT(*) FROM dbo.ChartOfAccounts WHERE IsActive = 1) AS TotalAccounts,
        
        (SELECT COUNT(*) FROM dbo.AspNetUsers WHERE EmailConfirmed = 1) AS ActiveUsers,
        
        (SELECT ISNULL(SUM(vd.DebitAmount), 0) 
         FROM dbo.VoucherMaster vm
         JOIN dbo.VoucherDetails vd ON vm.VoucherMasterID = vd.VoucherMasterID
         WHERE CONVERT(date, vm.VoucherDate) = CONVERT(date, GETUTCDATE())) AS TodaysTransactions,

        (SELECT COUNT(*) FROM dbo.AspNetUsers WHERE EmailConfirmed = 0) AS PendingApprovals;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetVoucherList
    @StartDate DATE = NULL,
    @EndDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        vm.VoucherMasterID,
        vm.VoucherDate,
        vm.VoucherType,
        vm.ReferenceNo,
        vm.Narration,
        SUM(vd.DebitAmount) AS TotalAmount
    FROM
        dbo.VoucherMaster vm
    JOIN
        dbo.VoucherDetails vd ON vm.VoucherMasterID = vd.VoucherMasterID
    WHERE
        (@StartDate IS NULL OR vm.VoucherDate >= @StartDate)
        AND
        (@EndDate IS NULL OR vm.VoucherDate <= @EndDate)
    GROUP BY
        vm.VoucherMasterID,
        vm.VoucherDate,
        vm.VoucherType,
        vm.ReferenceNo,
        vm.Narration
    ORDER BY
        vm.VoucherDate DESC, vm.VoucherMasterID DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_ManageChartOfAccounts
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
        UPDATE dbo.ChartOfAccounts 
        SET AccountName = @AccountName, AccountCode = @AccountCode, ParentAccountID = @ParentAccountID 
        WHERE AccountID = @AccountID;
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
GO

CREATE OR ALTER PROCEDURE dbo.sp_SaveVoucher
    @VoucherDate DATE,
    @VoucherType NVARCHAR(50),
    @ReferenceNo NVARCHAR(100),
    @Narration NVARCHAR(500),
    @CreatedBy NVARCHAR(450),
    @VoucherDetails dbo.VoucherDetailType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @VoucherMasterID BIGINT;

        INSERT INTO dbo.VoucherMaster (VoucherDate, VoucherType, ReferenceNo, Narration, CreatedBy)
        VALUES (@VoucherDate, @VoucherType, @ReferenceNo, @Narration, @CreatedBy);

        SET @VoucherMasterID = SCOPE_IDENTITY();

        INSERT INTO dbo.VoucherDetails (VoucherMasterID, AccountID, DebitAmount, CreditAmount)
        SELECT @VoucherMasterID, AccountID, DebitAmount, CreditAmount
        FROM @VoucherDetails;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        -- Re-throw the error to the calling application
        THROW;
    END CATCH
END
GO