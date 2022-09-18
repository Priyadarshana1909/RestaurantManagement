using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestaurantBLL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;
using RestaurantWebAPI.Controllers;
using System.Collections.Generic;

namespace RestaurantWebAPITest
{
    [TestClass]
    public class BillControllerUnitTest
    {
        private readonly Mock<IManageBillBLL> _mockManageBillBll;

        public BillControllerUnitTest()
        {
            _mockManageBillBll = new Mock<IManageBillBLL>();  
        }

        [TestMethod]
        public void BillControllerUnitTest_GetBill_SuccessResponse()
        {
            var response = new BillResponse();
            response.IsSuccessFull = true;
            response.Bills = new List<Bill>() {
                new Bill{ 
                    BillsID = 1,
                    BillAmount = 0
                }
            };
            _mockManageBillBll.Setup(x => x.GetBill(It.IsAny<int?>())).Returns(response);

            BillController billController = new BillController(_mockManageBillBll.Object);
            var BillResponse = billController.GetBill(0).Result;
            Assert.AreEqual(response.Bills.Count, BillResponse?.Value?.Bills.Count);
        }

        [TestMethod]
        public void BillControllerUnitTest_GetBill_FailResponse()
        {
            var response = new BillResponse();
            response.IsSuccessFull = false;
           
            _mockManageBillBll.Setup(x => x.GetBill(It.IsAny<int?>())).Returns(response);

            BillController billController = new BillController(_mockManageBillBll.Object);
            var BillResponse = billController.GetBill(0).Result;
            Assert.AreEqual(response.IsSuccessFull, BillResponse?.Value?.IsSuccessFull);
        }
    }
}