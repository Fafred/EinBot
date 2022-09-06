CREATE PROCEDURE [dbo].[spColumnDefinitions_Delete]
	@Id int
AS
BEGIN
	DELETE
	FROM [dbo].[ColumnDefinitions]
	WHERE Id = @Id;
END
