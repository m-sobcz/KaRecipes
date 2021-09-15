CREATE PROCEDURE [dbo].[Recipes_Get]
	@Id int
AS
SELECT * 
FROM Recipes
WHERE Recipes.Id=@Id
