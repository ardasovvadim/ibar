USE [IBAR.Master]
GO

/****** Object:  Table [dbo].[TradeInterestAccruas]    Script Date: 7/6/2020 12:53:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TradeInterestAccruas](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ReportDate] [datetime] NOT NULL,
	[EndingAccrualBalance] [decimal](18, 9) NOT NULL,
	[TradeAccountId] [bigint] NOT NULL,
	[Timestamp] [timestamp] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.TradeInterestAccruas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TradeInterestAccruas]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TradeInterestAccruas_dbo.TradeAccounts_TradeAccountId] FOREIGN KEY([TradeAccountId])
REFERENCES [dbo].[TradeAccounts] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[TradeInterestAccruas] CHECK CONSTRAINT [FK_dbo.TradeInterestAccruas_dbo.TradeAccounts_TradeAccountId]
GO


