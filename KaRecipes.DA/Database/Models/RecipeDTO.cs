using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.DA.Database.Models
{
    public class RecipeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? VersionId { get; set; }
    }
}
