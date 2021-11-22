using Xunit;
using KaRecipes.BL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaRecipes.BL.Interfaces;
using KaRecipes.BL.MachineStateAggregate;
using Moq;

namespace KaRecipes.BL.Services.Tests
{
    public class MachineStateServiceTests
    {
        [Fact()]
        public void LifeBitServiceTest_NormalOperation_Works()
        {
            //Arrange
            Mock<IPlcDataAccess> mockPlcDataAccess = new ();
            Mock<IDbDataAccess<MachineState>> mockDbDataAccess = new();
            MachineStateService machineStateService = new(mockPlcDataAccess.Object, mockDbDataAccess.Object);
            //Act
            Dictionary<string, MachineState> machineStates = new();
            machineStates.Add("KaRecipes.M01.DB_00_Parameters.single1", new MachineState() { NodeId = "KaRecipes.M01.DB_00_Parameters.single1" });
            machineStates.Add("KaRecipes.M01.DB_00_Parameters.single2", new MachineState() { NodeId = "KaRecipes.M01.DB_00_Parameters.single2" });
            machineStates.Add("KaRecipes.M01.DB_00_Parameters.single3", new MachineState() { NodeId = "KaRecipes.M01.DB_00_Parameters.single3" });
            machineStateService.Start(machineStates);
            machineStateService.Update(new PlcDataReceivedEventArgs() { Name = "KaRecipes.M01.DB_00_Parameters.single1", Value = "RUN" });
            machineStateService.Update(new PlcDataReceivedEventArgs() { Name = "KaRecipes.M01.DB_00_Parameters.single2", Value = "RUN" });
            machineStateService.Update(new PlcDataReceivedEventArgs() { Name = "KaRecipes.M01.DB_00_Parameters.single3", Value = "RUN" });
            //Assert
            mockPlcDataAccess.Verify(x => x.CreateSubscriptionsWithInterval(It.IsAny<List<string>>(),It.IsAny<int>(),It.IsAny<IObserver>()), Times.Once);
            mockDbDataAccess.Verify(x => x.Write(It.IsAny<MachineState>()),Times.Exactly(3));
        }
        [Fact()]
        public void LifeBitServiceTest_UnknownUpdateItem_ThrowsException()
        {
            //Arrange
            Mock<IPlcDataAccess> mockPlcDataAccess = new();
            Mock<IDbDataAccess<MachineState>> mockDbDataAccess = new();
            MachineStateService machineStateService = new(mockPlcDataAccess.Object, mockDbDataAccess.Object);
            //Act
            Dictionary<string, MachineState> machineStates = new();
            machineStateService.Start(machineStates);
            //Assert
            Assert.Throws<ArgumentException>(()=>machineStateService.Update(new PlcDataReceivedEventArgs() { Name = "UNKNOWN XXX", Value = "RUN" }));
        }
    }
}