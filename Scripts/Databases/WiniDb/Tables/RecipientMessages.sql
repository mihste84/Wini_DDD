CREATE TABLE [dbo].[RecipientMessages]
(
  [Id] INT IDENTITY(1,1) NOT NULL,
  [BookingId] INT NOT NULL,
  [Value] NVARCHAR(300) NOT NULL,
  [Recipient] NVARCHAR(200) NOT NULL,
  [CreatedBy] NVARCHAR(200) NOT NULL,
	[Created] DATETIME2(7) NOT NULL,
)
GO

ALTER TABLE [dbo].[RecipientMessages] ADD  CONSTRAINT [PK_RecipientMessages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
) 
GO

ALTER TABLE [dbo].[RecipientMessages] ADD CONSTRAINT DF_RecipientMessages_Created DEFAULT GETUTCDATE() FOR [Created];
GO

CREATE NONCLUSTERED INDEX [IX_RecipientMessages_BookingId] ON [dbo].[RecipientMessages]
(
	[BookingId] ASC
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_RecipientMessages_BookingId_Name_Size] ON [dbo].[RecipientMessages]
(
	[BookingId] ASC,
	[Recipient] ASC
)
WHERE ([BookingId] IS NOT NULL AND [Recipient] IS NOT NULL)
GO

ALTER TABLE [dbo].[RecipientMessages] ADD  CONSTRAINT [FK_RecipientMessages_Bookings_BookingId] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Bookings] ([Id])
ON DELETE CASCADE
GO
