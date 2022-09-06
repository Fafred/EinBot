CREATE PROCEDURE [dbo].[spDataTypes_Get]
	@Id int
AS
BEGIN
	SELECT *
	FROM [dbo].[DataTypes]
	WHERE Id = @Id;
END