CREATE TABLE [dbo].[Companies]
(
  [Id] INT IDENTITY(1,1) NOT NULL,
  [Code] NVARCHAR(5) NOT NULL,
  [Name] NVARCHAR(200) NOT NULL,
  [CountryCode] NVARCHAR(2) NOT NULL,
  [CreatedBy] NVARCHAR(200) NOT NULL,
	[Created] DATETIME2(7) NOT NULL,
	[UpdatedBy] NVARCHAR(200) NOT NULL,
	[Updated] DATETIME2(7) NOT NULL
)
GO

ALTER TABLE [dbo].[Companies] ADD  CONSTRAINT [PK_Companies] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
) 
GO

ALTER TABLE [dbo].[Companies] ADD CONSTRAINT DF_Companies_Created DEFAULT GETUTCDATE() FOR [Created];
GO

ALTER TABLE [dbo].[Companies] ADD CONSTRAINT DF_Companies_Updated DEFAULT GETUTCDATE() FOR [Updated];
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Companies_Code] ON [dbo].[Companies]
(
	[Code] ASC
)
GO
