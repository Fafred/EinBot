/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

IF NOT EXISTS (SELECT 1 FROM [dbo].[DataTypes])
BEGIN
    INSERT INTO [dbo].[DataTypes] ([Type])
    VALUES ('int'),
           ('decimal'),
           ('text'),
           ('UserID'),
           ('GuildID'),
           ('ChannelID'),
           ('ListInt'),
           ('ListDouble'),
           ('ListText'),
           ('ListUserID'),
           ('ListGuildID'),
           ('ListChannelID')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[CollectionTypes])
BEGIN
    INSERT INTO [dbo].[CollectionTypes] (TypeName)
    VALUES ('PerKey'),
           ('PerUser'),
           ('PerRole')
END