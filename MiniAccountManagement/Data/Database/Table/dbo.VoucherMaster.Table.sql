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