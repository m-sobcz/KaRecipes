using Xunit;
using KaRecipes.BL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaRecipes.BL.Interfaces;
using Moq;
using KaRecipes.BL.Data;

namespace KaRecipes.BL.Services.Tests
{
    public class LifeBitServiceTests
    {
        [Fact()]
        public void LifeBitServiceTest()
        {
            //Arrange
            Mock<IPlcDataAccess> mockPlcDataAccess = new Mock<IPlcDataAccess>();      
            LifeBitService lifeBitService;
            List<object> writtenVals = new();
            mockPlcDataAccess.Setup(x => x.WriteDataNodes(It.IsAny<List<DataNode>>())).Callback<List<DataNode>>(x=>writtenVals.AddRange(x.Select(x=>x.Value)));//x => writtenNodes.AddRange(x)
            lifeBitService = new(mockPlcDataAccess.Object);
            //Act
            List<DataNode> dataNodes = new();
            dataNodes.Add(new DataNode() { NodeId = "KaRecipes.M01.DB_00_Parameters.single1" });
            dataNodes.Add(new DataNode() { NodeId = "KaRecipes.M01.DB_00_Parameters.single2" });
            lifeBitService.Interval = 100;
            lifeBitService.Start(dataNodes);
            Task.Delay(200).Wait();
            lifeBitService.Stop();
            //Assert
            Assert.Equal(4, writtenVals.Count);
            Assert.False(writtenVals[0] as bool?);
            Assert.False(writtenVals[1] as bool?);
            Assert.True(writtenVals[2] as bool?);
            Assert.True(writtenVals[3] as bool?);
        }
    }
}