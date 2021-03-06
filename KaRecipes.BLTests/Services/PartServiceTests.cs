using Xunit;
using KaRecipes.BL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaRecipes.BL.Interfaces;
using Moq;
using KaRecipes.BL.Part;
using KaRecipes.BL.Data.RequestAggregate;
using KaRecipes.BL.Data.PartAggregate;

namespace KaRecipes.BL.Services.Tests
{
    public class PartServiceTests
    {
        [Fact()]
        public void PartDataServiceTest_RequestStart_OnceAfter()
        {
            //Arrange
            Mock<IPlcDataAccess> mockPlcDataAccess = new();
            Mock<IRequest> mockRequest = new();
            mockRequest.SetupGet(x => x.Command).Returns(new RequestData() { NodeId = "KaRecipes.M01.DB_00_Parameters.singleC", ParentRequest=mockRequest.Object });
            mockRequest.SetupGet(x => x.PartId).Returns(new RequestData() { NodeId = "KaRecipes.M01.DB_00_Parameters.partId" });

            PartData partData = new();
            partData.DataNodes = new();
            partData.DataNodes.Add(new RequestData() {NodeId="KaRecipes.M01.DB_00_Parameters.single1" });
            partData.DataNodes.Add(new RequestData() { NodeId = "KaRecipes.M01.DB_00_Parameters.single2" });
            mockRequest.SetupGet(x => x.Data).Returns(()=>partData);
            PartService partDataService = new(mockPlcDataAccess.Object);
            List<IRequest> requests = new();
            requests.Add(mockRequest.Object);

            //Act
            partDataService.Start(requests);
            PlcDataReceivedEventArgs plcDataReceivedEventArgs = new() { Name = "KaRecipes.M01.DB_00_Parameters.singleC", Value = true };
            partDataService.Update(plcDataReceivedEventArgs);
            partDataService.Update(plcDataReceivedEventArgs);
            partDataService.Update(plcDataReceivedEventArgs);
            //Assert
            mockPlcDataAccess.Verify(x => x.CreateSubscriptionsWithInterval(It.IsAny<List<string>>(), It.IsAny<int>(), It.IsAny<IObserver>()), Times.Once);
            mockRequest.Verify(x => x.ExecuteStart(It.IsAny<PartData>()), Times.Once);
        }
    }
}