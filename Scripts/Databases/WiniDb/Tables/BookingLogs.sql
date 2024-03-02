CREATE TABLE [dbo].[BookingStatusLogs]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [BookingId] INT NOT NULL,
    [Status] SMALLINT NOT NULL,
    [CreatedBy] NVARCHAR(200) NOT NULL,
    [Created] DATETIME2(7) NOT NULL,
)
GO

ALTER TABLE [dbo].[BookingStatusLogs] ADD  CONSTRAINT [PK_BookingStatusLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
) 
GO

ALTER TABLE [dbo].[BookingStatusLogs] ADD CONSTRAINT DF_BookingStatusLogs_Created DEFAULT GETUTCDATE() FOR [Created];
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_BookingStatusLogs_BookingId] ON [dbo].[BookingStatusLogs]
(
	[BookingId] ASC,
	[Status] ASC,
	[Created] ASC
)
GO

ALTER TABLE [dbo].[BookingStatusLogs] ADD  CONSTRAINT [FK_BookingStatusLogs_Bookings_BookingId] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Bookings] ([Id])
ON DELETE CASCADE
GO
