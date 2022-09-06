CREATE PROCEDURE [dbo].[spColumnDefinitions_Insert]
	@TableDefinitionsId int,
	@ColumnName nvarchar(50),
	@DataTypesId int
AS
BEGIN
	INSERT INTO [dbo].[ColumnDefinitions] ([TableDefinitionsId], [ColumnName], [DataTypesId])
	VALUES (@TableDefinitionsId, @ColumnName, @DataTypesId);
END
