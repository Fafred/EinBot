CREATE PROCEDURE [dbo].[spCells_UpdateDataByTableColumnKey]
	@TableDefinitionsId INT,
	@ColumnDefinitionsId INT,
	@Key NVARCHAR(50),
	@Data NVARCHAR(50)
AS
	UPDATE [dbo].[Cells]
	SET [Data] = @Data
	WHERE TableDefinitionsId = @TableDefinitionsId AND ColumnDefinitionsId = @ColumnDefinitionsId AND [Key] = @Key
