-- This file contains SQL statements that will be executed after the build script.
SET IDENTITY_INSERT [dbo].[Companies] ON;

IF NOT EXISTS (SELECT * FROM [dbo].[Companies])
    INSERT INTO [dbo].[Companies] ([Id], [Code], [Name], [CountryCode], [Created], [CreatedBy], [Updated], [UpdatedBy])
    VALUES (1, '100', 'If skadeförsäkringar SE', 'SE', GETUTCDATE(), 'SEED_SCRIPT', GETUTCDATE(), 'SEED_SCRIPT'),
        (2, '200', 'If skadeförsäkringar NO', 'NO', GETUTCDATE(), 'SEED_SCRIPT', GETUTCDATE(), 'SEED_SCRIPT'),
        (3, '300', 'If skadeförsäkringar DK', 'DK', GETUTCDATE(), 'SEED_SCRIPT', GETUTCDATE(), 'SEED_SCRIPT'),
        (4, '400', 'If skadeförsäkringar FI', 'FI', GETUTCDATE(), 'SEED_SCRIPT', GETUTCDATE(), 'SEED_SCRIPT');
ELSE 
    print 'Companies already seeded';

SET IDENTITY_INSERT [dbo].[Companies] OFF;    

GO

