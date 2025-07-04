IF TYPE_ID(N'dbo.VoucherDetailType') IS NOT NULL
    DROP TYPE dbo.VoucherDetailType;
GO

CREATE TYPE dbo.VoucherDetailType AS TABLE(
    AccountID INT NOT NULL,
    DebitAmount DECIMAL(18, 2) NOT NULL,
    CreditAmount DECIMAL(18, 2) NOT NULL
);
GO