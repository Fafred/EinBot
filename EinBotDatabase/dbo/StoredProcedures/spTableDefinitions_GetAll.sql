CREATE PROCEDURE [dbo].[spTableDefinitions_Get]
	@Id int
AS
BEGIN
	SELECT *
	FROM [TableDefinitions]
	WHERE Id = @Id;
END
