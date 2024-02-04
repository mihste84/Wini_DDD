CREATE TABLE [dbo].[BookingRows]
(
  [Id] INT IDENTITY(1,1) NOT NULL,
  [BookingId] INT NOT NULL,
  [RowNumber] INT NOT NULL,
  [BusinessUnit] NVARCHAR(12) NULL,
  [Account] NVARCHAR(6) NULL,
  [Subsidiary] NVARCHAR(8) NULL,
  [Subledger] NVARCHAR(8) NULL,
  [SubledgerType] NVARCHAR(1) NULL,
  [CostObject1] NVARCHAR(12) NULL,
  [CostObjectType1] NVARCHAR(1) NULL,
  [CostObject2] NVARCHAR(12) NULL,
  [CostObjectType2] NVARCHAR(1) NULL,
  [CostObject3] NVARCHAR(12) NULL,
  [CostObjectType3] NVARCHAR(1) NULL,
  [CostObject4] NVARCHAR(12) NULL,
  [CostObjectType4] NVARCHAR(1) NULL,
  [Remark] NVARCHAR(30) NULL,
  [Authorizer] NVARCHAR(200) NULL,
  [IsAuthorized] BIT NOT NULL,
  [Amount] Decimal(12, 2) NULL,
  [CurrencyCode] NVARCHAR(3) NULL,
  [ExchangeRate] Decimal(9, 6) NULL
)
GO

ALTER TABLE [dbo].[BookingRows] ADD  CONSTRAINT [PK_BookingRows] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
) 
GO

ALTER TABLE [dbo].[BookingRows] ADD CONSTRAINT DF_BookingRows_IsAuthorized DEFAULT 0 FOR [IsAuthorized];
GO

CREATE NONCLUSTERED INDEX [IX_BookingRows_BookingId] ON [dbo].[BookingRows]
(
	[BookingId] ASC
)
GO

ALTER TABLE [dbo].[BookingRows] ADD  CONSTRAINT [FK_BookingRows_Bookings_BookingId] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Bookings] ([Id])
ON DELETE CASCADE
GO