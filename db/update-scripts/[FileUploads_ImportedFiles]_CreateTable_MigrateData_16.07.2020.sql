-- create new table 
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FileUploads](
	[ImportedFileId] [bigint] NOT NULL,
	[IsSent] [bit] NOT NULL,
	[Timestamp] [timestamp] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.FileUploads] PRIMARY KEY CLUSTERED 
(
	[ImportedFileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FileUploads]  WITH CHECK ADD  CONSTRAINT [FK_dbo.FileUploads_dbo.ImportedFiles_ImportedFileId] FOREIGN KEY([ImportedFileId])
REFERENCES [dbo].[ImportedFiles] ([Id])
GO
ALTER TABLE [dbo].[FileUploads] CHECK CONSTRAINT [FK_dbo.FileUploads_dbo.ImportedFiles_ImportedFileId]
GO

-- migration data from ImportedFiles
INSERT INTO FileUploads (ImportedFileId, IsSent, CreatedDate, ModifiedDate, Deleted) 
SELECT	Id, 
		IsSent, 
		CreatedDate, 
		CASE 
			WHEN IsSent = 0 THEN NULL
			ELSE ModifiedDate 
		END, 
		Deleted 
FROM ImportedFiles
WHERE IsForApi = 1
GO

-- remove unnecessary columns
ALTER TABLE ImportedFiles 
DROP COLUMN IsSent, IsForApi