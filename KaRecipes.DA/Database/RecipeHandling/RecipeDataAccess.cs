using KaRecipes.BL.Interfaces;
using KaRecipes.BL.RecipeAggregate;
using KaRecipes.DA.Database.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.DA.Database.RecipeHandling
{
    class RecipeDataAccess : IDbDataAccess<Recipe>
    {
        ISqlDataAccess sqlDataAccess;
        public RecipeDataAccess(ISqlDataAccess sqlDataAccess)
        {
            this.sqlDataAccess = sqlDataAccess;
        }
        public Task<Recipe> Read()
        {
            throw new NotImplementedException();
        }

        public Task<int?> Write(Recipe data)
        {
            throw new NotImplementedException();
        }
    }
}
