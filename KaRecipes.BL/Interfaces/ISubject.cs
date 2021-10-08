using KaRecipes.BL.RecipeAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.Interfaces
{
    public interface ISubject
    {
        ParameterSingle ParameterSingle { get; set; }
    }
}
