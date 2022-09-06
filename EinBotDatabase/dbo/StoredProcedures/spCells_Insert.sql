CREATE PROCEDURE [dbo].[spCells_Insert]
	@TableDefinitionsId INT,
	@ColumnDefinitionsId INT,
	@RowNum INT,
	@Key NVARCHAR(50),
	@Data NVARCHAR(50)
AS
BEGIN
	INSERT INTO [dbo].[Cells] ([TableDefinitionsId], [ColumnDefinitionsId], [RowNum], [Data], [RowKey])
	VALUES (@TableDefinitionsId, @ColumnDefinitionsId, @RowNum, @Data, @Key);
END
