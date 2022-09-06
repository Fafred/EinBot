CREATE PROCEDURE [dbo].[spTableDefinitions_Delete]
	@Id INT
AS
BEGIN
	DELETE
	FROM [dbo].[TableDefinitions]
	WHERE Id = @Id;
END