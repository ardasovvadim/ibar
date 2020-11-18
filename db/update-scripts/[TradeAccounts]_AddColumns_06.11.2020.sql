-- IT-491
ALTER TABLE [IBAR.Master].[dbo].[TradeAccounts] 
ADD StateResidentialAddress nvarchar(max) NULL, IbEntity nvarchar(max) NULL, AccountType nvarchar(max)  NULL
GO
 
 UPDATE [IBAR.Master].[dbo].[ImportedFiles]
 SET FileState = 0, FileStatus = 0
 WHERE OriginalFileName LIKE '%acct_status%' OR OriginalFileName LIKE '%Client_info%'