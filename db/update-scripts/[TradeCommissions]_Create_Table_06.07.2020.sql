USE [IBAR.Master]
GO

/****** Object:  Table [dbo].[TradeCommissions]    Script Date: 7/6/2020 12:54:34 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TradeCommissions](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ReportDate] [datetime] NOT NULL,
	[FxRateToBase] [decimal](18, 9) NOT NULL,
	[TotalCommission] [decimal](18, 9) NOT NULL,
	[TradeAccountId] [bigint] NOT NULL,
	[Timestamp] [timestamp] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.TradeCommissions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TradeCommissions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TradeCommissions_dbo.TradeAccounts_TradeAccountId] FOREIGN KEY([TradeAccountId])
REFERENCES [dbo].[TradeAccounts] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[TradeCommissions] CHECK CONSTRAINT [FK_dbo.TradeCommissions_dbo.TradeAccounts_TradeAccountId]
GO


