CREATE TABLE [dbo].[Comments]
(
  [Id] INT IDENTITY(1,1) NOT NULL,
  [BookingId] INT NOT NULL,
  [Value] NVARCHAR(300) NOT NULL,
  [CreatedBy] NVARCHAR(200) NOT NULL,
	[Created] DATETIME2(7) NOT NULL,
)
GO

ALTER TABLE [dbo].[Comments] ADD  CONSTRAINT [PK_Comments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
) 
GO

ALTER TABLE [dbo].[Comments] ADD CONSTRAINT DF_Comments_Created DEFAULT GETUTCDATE() FOR [Created];
GO

CREATE NONCLUSTERED INDEX [IX_Comments_BookingId] ON [dbo].[Comments]
(
	[BookingId] ASC
)
GO

ALTER TABLE [dbo].[Comments] ADD  CONSTRAINT [FK_Comments_Bookings_BookingId] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Bookings] ([Id])
ON DELETE CASCADE
GO
