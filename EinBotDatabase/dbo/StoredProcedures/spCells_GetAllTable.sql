CREATE PROCEDURE [dbo].[spCells_GetAllTable]
	@TableDefinitionsId int
AS
BEGIN
	SELECT *
	FROM [dbo].[Cells]
	WHERE TableDefinitionsId = @TableDefinitionsId;
END