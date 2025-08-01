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