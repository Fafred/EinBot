CREATE PROCEDURE [dbo].[spColumnDefinitions_GetTableColumns]
	@TableId int
AS
BEGIN
	SELECT *
	FROM ColumnDefinitions
	WHERE TableDefinitionsId = @TableId;
END
