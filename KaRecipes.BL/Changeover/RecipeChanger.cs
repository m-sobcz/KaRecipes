using KaRecipes.BL.Interfaces;
using KaRecipes.BL.RecipeAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.Changeover
{
    public class RecipeChanger
    {
        readonly string plcAccessPrefix = "KaRecipes.";
        IPlcDataAccess plcDataAccess;
        Recipe RecipeTemplate { get; set; }
        public RecipeChanger(IPlcDataAccess plcDataAccess)
        {
            this.plcDataAccess = plcDataAccess;
        }
        

    }
}
