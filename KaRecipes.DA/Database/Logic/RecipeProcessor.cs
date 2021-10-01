using KaRecipes.DA.Database.DataAccess;
using KaRecipes.DA.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaRecipes.DA.Database.Logic
{
    public class RecipeProcessor : Processor
    {
        private readonly IDbDataAccess dataAccess;
        //Nazwy wywolywanych stored procedures: Recipes_<nazwa_funkcji>,
        //np. Recipes_Create
        public RecipeProcessor(IDbDataAccess dataAccess)
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
            return dataAccess.Load<RecipeDTO>(GetDefaultsql(), parameter).FirstOrDefault();
        }
        public RecipeDTO GetAll()
        {
            return dataAccess.Load<RecipeDTO>(GetDefaultsql()).FirstOrDefault();
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
