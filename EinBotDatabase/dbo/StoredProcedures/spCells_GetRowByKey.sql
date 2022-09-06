CREATE PROCEDURE [dbo].[spCells_GetRowByKey]
	@TableDefinitionsId INT,
	@Key NVARCHAR(50)
AS
BEGIN
	SELECT *
	FROM [dbo].[Cells]
	WHERE TableDefinitionsId = @TableDefinitionsId AND [RowKey] = @Key
END