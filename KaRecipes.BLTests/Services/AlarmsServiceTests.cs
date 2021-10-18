using Xunit;
using KaRecipes.BL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaRecipes.BL.Interfaces;
using Moq;
using KaRecipes.BL.AlarmAggregate;

namespace KaRecipes.BL.Services.Tests
{
    public class AlarmsServiceTests
    {


        [Fact()]
        public void Update_SampleData_UpdatesValue()
        {
            //Arrange
            Mock<IPlcDataAccess> mockPlcDataAccess = new Mock<IPlcDataAccess>();
            Dictionary<string, Alarm> alarms = new();
            alarms.Add("KaRecipes.M01.DB_00_Parameters.single1", new Alarm() {Value=false });
            alarms.Add("KaRecipes.M01.DB_00_Parameters.single2", new Alarm() { Value = false });
            alarms.Add("KaRecipes.M01.DB_00_Parameters.single3", new Alarm() { Value = false });
            AlarmsService alarmsService = new(mockPlcDataAccess.Object);
            //Act
            alarmsService.Start(alarms);
            alarmsService.Update(new PlcDataReceivedEventArgs() { Name = "KaRecipes.M01.DB_00_Parameters.single2",Value=true });
            var alarm1 = alarms.GetValueOrDefault("KaRecipes.M01.DB_00_Parameters.single1");
            var alarm2 = alarms.GetValueOrDefault("KaRecipes.M01.DB_00_Parameters.single2");
            var alarm3 = alarms.GetValueOrDefault("KaRecipes.M01.DB_00_Parameters.single3");
            //Assert
            Assert.Equal(false, alarm1.Value);
            Assert.Equal(true, alarm2.Value);
            Assert.Equal(false, alarm3.Value);
        }
    }
}