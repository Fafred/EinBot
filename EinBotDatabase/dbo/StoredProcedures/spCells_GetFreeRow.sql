CREATE PROCEDURE [dbo].[spCells_GetFreeRow]
	@TableDefinitionsId INT
AS
BEGIN
	SELECT MAX([RowNum]) + 1 as FreeRow
	FROM [dbo].[Cells]
	WHERE TableDefinitionsId = @TableDefinitionsId;
END