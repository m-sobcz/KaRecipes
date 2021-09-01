using Xunit;
using KaRecipes.UI.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.UI.Converters.Tests
{
    public class IntToStringConverterTests
    {
        [Theory]
        [InlineData(5, "5")]
        [InlineData(-101, "-101")]
        public void ConvertTest(int input, string expected)
        {
            IntToStringConverter intToStringConverter= new();
            object actual = intToStringConverter.Convert(input, typeof(string), null, System.Globalization.CultureInfo.InvariantCulture);
            Assert.Equal(expected, actual.ToString());
        }
        [Theory]
        [InlineData("666",666)]
        [InlineData("-1",-1)]
        public void ConvertBackTest(string input, int expected)
        {
            IntToStringConverter intToStringConverter = new();
            int convertedBack = (int)intToStringConverter.ConvertBack(input, typeof(int), null, System.Globalization.CultureInfo.InvariantCulture);
            Assert.Equal(expected, convertedBack);
        }


    }
}