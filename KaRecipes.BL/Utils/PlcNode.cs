using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KaRecipes.BL.Utils
{
    static class PlcNode
    {
        readonly static string plcAccessPrefix = "KaRecipes";
        readonly static Regex stationRegex = new(@"DB.+", RegexOptions.Compiled);
        public static string GetNodeIdentifier(string module, string station, string parameter)
        {
            string stationName = stationRegex.Match(station).Value;
            string path = $"{plcAccessPrefix}.{module}.{stationName}.{parameter}";
            return path;
        }
    }
}
