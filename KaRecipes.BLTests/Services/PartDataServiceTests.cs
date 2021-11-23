using Xunit;
using KaRecipes.BL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaRecipes.BL.Interfaces;
using Moq;
using KaRecipes.BL.PlcRequest;

namespace KaRecipes.BL.Services.Tests
{
    public class PartDataServiceTests
    {
        [Fact()]
        public void PartDataServiceTest_RequestStart_OnceAfter()
        {
            //Arrange
            Mock<IPlcDataAccess> mockPlcDataAccess = new();
            Mock<IRequest> mockRequest = new();
            mockRequest.SetupGet(x => x.Command).Returns(new RequestData() { NodeId = "KaRecipes.M01.DB_00_Parameters.singleC", ParentRequest=mockRequest.Object });
            mockRequest.SetupGet(x => x.PartId).Returns(new RequestData() { NodeId = "KaRecipes.M01.DB_00_Parameters.partId" });

            var data = new Dictionary<string, RequestData>();
            data.Add("KaRecipes.M01.DB_00_Parameters.single1", new RequestData() { });
            data.Add("KaRecipes.M01.DB_00_Parameters.single2", new RequestData() { });
            mockRequest.SetupGet(x => x.Data).Returns(()=>data);
            PartDataService partDataService = new(mockPlcDataAccess.Object);
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
            mockRequest.Verify(x => x.ExecuteStart(), Times.Once);
        }
    }
}