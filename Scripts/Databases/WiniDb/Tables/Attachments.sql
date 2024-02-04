CREATE TABLE [dbo].[Attachments]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[BookingId] INT NOT NULL,
	[Size] INT NOT NULL,
	[ContentType] NVARCHAR(200) NOT NULL,
	[Name] NVARCHAR(300) NOT NULL,
	[Path] NVARCHAR(400) NOT NULL,
	[CreatedBy] NVARCHAR(200) NOT NULL,
	[Created] DATETIME2(7) NOT NULL
)
GO

ALTER TABLE [dbo].[Attachments] ADD  CONSTRAINT [PK_Attachments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
) 
GO


ALTER TABLE [dbo].[Attachments] ADD CONSTRAINT DF_Attachments_Created DEFAULT GETUTCDATE() FOR [Created];
GO

CREATE NONCLUSTERED INDEX [IX_Attachments_BookingId] ON [dbo].[Attachments]
(
	[BookingId] ASC
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Attachments_BookingId_Name_Size] ON [dbo].[Attachments]
(
	[BookingId] ASC,
	[Name] ASC,
	[Size] ASC
)
WHERE ([BookingId] IS NOT NULL AND [Name] IS NOT NULL AND [Size] IS NOT NULL)
GO

ALTER TABLE [dbo].[Attachments] ADD  CONSTRAINT [FK_Attachments_Bookings_BookingId] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Bookings] ([Id])
ON DELETE CASCADE
GO
