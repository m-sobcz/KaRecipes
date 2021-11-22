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
            Mock<IDbDataAccess<Alarm>> mockDbDataAccess = new();
            List<Alarm> alarmsSentToDbWrite = new();
            mockDbDataAccess.Setup(x => x.Write(It.IsAny<Alarm>())).Callback<Alarm>(
                x => alarmsSentToDbWrite.Add(x));
            Dictionary<string, Alarm> alarms = new();
            alarms.Add("KaRecipes.M01.DB_00_Parameters.single1", new Alarm() { NodeId = "KaRecipes.M01.DB_00_Parameters.single1" });
            alarms.Add("KaRecipes.M01.DB_00_Parameters.single2", new Alarm() { NodeId = "KaRecipes.M01.DB_00_Parameters.single2" });
            alarms.Add("KaRecipes.M01.DB_00_Parameters.single3", new Alarm() { NodeId = "KaRecipes.M01.DB_00_Parameters.single3" });
            AlarmsService alarmsService = new(mockPlcDataAccess.Object, mockDbDataAccess.Object);
            //Act
            alarmsService.Start(alarms);
            alarmsService.Update(new PlcDataReceivedEventArgs() { Name = "KaRecipes.M01.DB_00_Parameters.single2", Value = true });
            alarmsService.Update(new PlcDataReceivedEventArgs() { Name = "KaRecipes.M01.DB_00_Parameters.single2", Value = true });
            var alarm1 = alarms.GetValueOrDefault("KaRecipes.M01.DB_00_Parameters.single1");
            var alarm2 = alarms.GetValueOrDefault("KaRecipes.M01.DB_00_Parameters.single2");
            var alarm3 = alarms.GetValueOrDefault("KaRecipes.M01.DB_00_Parameters.single3");
            //Assert
            Assert.Null(alarm1.Value);
            Assert.Equal(true, alarm2.Value);
            Assert.Null(alarm3.Value);
            Assert.Single(alarmsSentToDbWrite);
            Assert.Equal("KaRecipes.M01.DB_00_Parameters.single2", alarmsSentToDbWrite.FirstOrDefault().NodeId);
        }
    }
}