CREATE TABLE [dbo].[Events](
		[Id]               [bigint] IDENTITY(1, 1) NOT NULL,
		[AggregateId]      [uniqueidentifier] NOT NULL,
		[EventData]        [nvarchar](max) NOT NULL,
		[Version]		   [int] not null default 0,
		[CreatedOn]        [datetime2] DEFAULT GETDATE() NOT NULL,	
		[Type]			   [varchar](max) not null
	PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	)
)