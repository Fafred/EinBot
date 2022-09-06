CREATE PROCEDURE [dbo].[spTableDefinitions_Update]
	@Id INT,
	@TableName NVARCHAR(50),
	@CollectionTypesId INT,
	@RoleId INT
AS
BEGIN
	UPDATE [dbo].[TableDefinitions]
	SET TableName = @TableName, CollectionTypesId = @CollectionTypesId, RoleId = @RoleId
	WHERE Id = @Id;
END