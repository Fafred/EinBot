CREATE PROCEDURE [dbo].[spCollectionTypes_Get]
	@Id int = 0
AS
BEGIN
	SELECT *
	FROM [dbo].[CollectionTypes]
	WHERE Id = @Id;
END