using KaRecipes.BL.Data.RecipeAggregate;
using KaRecipes.BL.Interfaces;
using KaRecipes.DA.Database.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.DA.Database.RecipeHandling
{
    class RecipeDataAccess : IDbWrite<RecipeData>
    {
        ISqlDataAccess sqlDataAccess;
        public RecipeDataAccess(ISqlDataAccess sqlDataAccess)
        {
            this.sqlDataAccess = sqlDataAccess;
        }


        public Task<int?> Write(RecipeData data)
        {
            throw new NotImplementedException();
        }

        Task<RecipeData> IDbWrite<RecipeData>.Write(RecipeData data)
        {
            throw new NotImplementedException();
        }
    }
}
