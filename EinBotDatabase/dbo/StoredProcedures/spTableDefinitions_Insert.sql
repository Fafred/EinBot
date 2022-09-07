CREATE PROCEDURE [dbo].[spTableDefinitions_Insert]
	@TableName nvarchar(50),
	@CollectionTypeId int,
	@RoleId nvarchar(50)
AS
BEGIN
	INSERT INTO [dbo].[TableDefinitions] ([TableName], [CollectionTypesId], [RoleId])
	VALUES (@TableName, @CollectionTypeId, @RoleId)
END
