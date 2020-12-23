
CREATE TABLE [dbo].[PaymentProjections]( 
	[PaymentId]	  [uniqueidentifier] NOT NULL,
	[MerchantId]	  [uniqueidentifier] NOT NULL,
	[CardNumber]	  [nvarchar](16) NOT NULL,
	[CardExpiry]	  [nvarchar](10) NOT NULL,
	[CardCvv]		  [nvarchar](3) NOT NULL, 
	[Amount]		  [float] NOT Null,
	[Currency]		  [nvarchar](30) NOT NULL,
	[PaymentStatus]	  [nvarchar](30) NOT null, 
	[LastUpdatedDate] [datetime2](7) NOT NULL,
	[AcquiringBankId] [uniqueidentifier] NULL,
	[FailedDetails]	  [nvarchar](max) null, 
	PRIMARY KEY CLUSTERED
	(
		[PaymentId] ASC
	)
)


