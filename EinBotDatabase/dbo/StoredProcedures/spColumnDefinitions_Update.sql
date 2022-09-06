CREATE PROCEDURE [dbo].[spColumnDefinitions_Update]
	@Id INT,
	@TableDefinitionsId INT,
	@ColumnName NVARCHAR(50),
	@DataTypesId INT
AS
BEGIN
	UPDATE [dbo].[ColumnDefinitions]
	SET TableDefinitionsId = @TableDefinitionsId, ColumnName = @ColumnName, DataTypesId = @DataTypesId
	WHERE Id = @Id;
END
