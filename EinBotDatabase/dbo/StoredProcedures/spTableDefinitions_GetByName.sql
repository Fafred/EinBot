CREATE PROCEDURE [dbo].[spTableDefinitions_GetByName]
	@TableName NVARCHAR(50)
AS
BEGIN
	SELECT *
	FROM [TableDefinitions]
	WHERE TableName = @TableName;
END
