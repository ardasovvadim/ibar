USE [IBAR.Master]
GO

/****** Object:  Table [dbo].[TradesExes]    Script Date: 6/25/2020 1:55:30 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TradesExes](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Quantity] [decimal](18, 9) NULL,
	[TradePrice] [decimal](18, 9) NULL,
	[ClosePrice] [decimal](18, 9) NULL,
	[VolatilityOrderLink] [bigint] NULL,
	[AssetCategory] [nvarchar](max) NULL,
	[PutCall] [nvarchar](max) NULL,
	[BuySell] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[OrderType] [nvarchar](max) NULL,
	[OrderReference] [nvarchar](max) NULL,
	[Currency] [nvarchar](max) NULL,
	[IbExecId] [nvarchar](max) NULL,
	[IsAPIOrder] [nvarchar](max) NULL,
	[OpenCloseIndicator] [nvarchar](max) NULL,
	[Isin] [nvarchar](max) NULL,
	[Multiplier] [decimal](18, 9) NULL,
	[FxRateToBase] [decimal](18, 9) NULL,
	[TransactionID] [bigint] NULL,
	[IbOrderID] [bigint] NULL,
	[IbCommission] [decimal](18, 9) NULL,
	[Conid] [decimal](18, 9) NULL,
	[Strike] [decimal](18, 9) NULL,
	[Taxes] [decimal](18, 9) NULL,
	[Expiry] [datetime] NULL,
	[SettleDateTarget] [datetime] NULL,
	[OrderTime] [datetime] NULL,
	[ReportDate] [datetime] NULL,
	[Symbol] [nvarchar](max) NULL,
	[ListingExchange] [nvarchar](max) NULL,
	[UnderlyingSymbol] [nvarchar](max) NULL,
	[TradeAccountId] [bigint] NOT NULL,
	[Timestamp] [timestamp] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.TradesExes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[TradesExes]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TradesExes_dbo.TradeAccounts_TradeAccountId] FOREIGN KEY([TradeAccountId])
REFERENCES [dbo].[TradeAccounts] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[TradesExes] CHECK CONSTRAINT [FK_dbo.TradesExes_dbo.TradeAccounts_TradeAccountId]
GO


