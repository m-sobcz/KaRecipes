using KaRecipes.BL.Interfaces;
using KaRecipes.BL.PartAggregate;
using KaRecipes.DA.Database.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.DA.Database.PartDataHandling
{
    public class PartDataAccess : IDbDataAccess<PartData>
    {
        ISqlDataAccess sqlDataAccess;
        public PartDataAccess(ISqlDataAccess sqlDataAccess)
        {
            this.sqlDataAccess = sqlDataAccess;
        }
        public Task<PartData> Read(object id)
        {
            throw new NotImplementedException();
        }

        public Task<int?> Write(PartData data)
        {
            throw new NotImplementedException();
        }
    }
}
