CREATE TABLE [dbo].[Bookings]
(
  [Id] INT IDENTITY(1,1) NOT NULL,
  [Status] SMALLINT NOT NULL,
  [BookingDate] DATETIME2 NOT NULL,
  [TextToE1] NVARCHAR(30) NULL,
  [Reversed] bit NOT NULL,
  [LedgerType] SMALLINT NOT NULL,
  [CreatedBy] NVARCHAR(200) NOT NULL,
	[Created] DATETIME2(7) NOT NULL,
	[UpdatedBy] NVARCHAR(200) NOT NULL,
	[Updated] DATETIME2(7) NOT NULL,
  [RowVersion] ROWVERSION NOT NULL
)
GO

ALTER TABLE [dbo].[Bookings] ADD  CONSTRAINT [PK_Bookings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
) 
GO

ALTER TABLE [dbo].[Bookings] ADD CONSTRAINT DF_Bookings_Created DEFAULT GETUTCDATE() FOR [Created];
GO

ALTER TABLE [dbo].[Bookings] ADD CONSTRAINT DF_Bookings_Updated DEFAULT GETUTCDATE() FOR [Updated];
GO

CREATE NONCLUSTERED INDEX [IX_Bookings_BookingDate_Status] ON [dbo].[Bookings]
(
    [BookingDate] ASC,
    [Status] ASC
)
GO