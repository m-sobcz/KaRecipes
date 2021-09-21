using KaRecipes.DA;
using KaRecipes.DA.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CookBookBLL.Logic
{
    public class RecipeProcessor : Processor
    {
        private readonly IDataAccess dataAccess;
        //Nazwy wywolywanych stored procedures: Recipes_<nazwa_funkcji>,
        //np. Recipes_Create
        public RecipeProcessor(IDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
            defaultStoredProceduresPrefix = "Recipes";
        }
        public int? Create(RecipeDTO recipeModel)
        {
            return dataAccess.Save(GetDefaultsql(), recipeModel);
        }
        public int? Update(RecipeDTO recipeModel)
        {
            return dataAccess.Save(GetDefaultsql(), recipeModel);
        }
        public RecipeDTO Get(int id)
        {
            var parameter = new
            {
                Id = id
            };
            return dataAccess.Load<Recipe>(GetDefaultsql(), parameter).FirstOrDefault();
        }
        public RecipeDTO GetAll()
        {
            return dataAccess.Load<Recipe>(GetDefaultsql()).FirstOrDefault();
        }
        public int Delete(int id)
        {
            var parameter = new
            {
                Id = id
            };
            return dataAccess.Delete(GetDefaultsql(), parameter);
        }
    }
}
