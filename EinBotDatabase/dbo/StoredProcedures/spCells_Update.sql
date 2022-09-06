CREATE PROCEDURE [dbo].[spCells_Update]
	@Id INT,
	@TableDefinitionsId INT,
	@ColumnDefinitionsId INT,
	@RowNum INT,
	@Key NVARCHAR(50),
	@Data NVARCHAR(50)
AS
BEGIN
	UPDATE [dbo].[Cells]
	SET TableDefinitionsId = @TableDefinitionsId, ColumnDefinitionsId = @ColumnDefinitionsId, RowNum = @RowNum, [RowKey] = @Key, [Data] = @Data
	WHERE Id = @Id;
END
