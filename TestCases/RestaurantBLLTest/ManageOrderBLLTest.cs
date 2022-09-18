using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestaurantBLL;
using RestaurantDAL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantBLLTest
{
    [TestClass]
    public class ManageOrderBLLTest
    {
        private readonly Mock<IManageOrderDAL> _mockManageOrderDAL;

        public ManageOrderBLLTest()
        {
            _mockManageOrderDAL = new Mock<IManageOrderDAL>();
        }

        [TestMethod]
        public void ManageOrderBLLTest_GetCuisines_SuccessResponse()
        {
            var response = new OrderResponse();
            response.IsSuccessFull = true;

            _mockManageOrderDAL.Setup(x => x.GetOrder(It.IsAny<int?>())).Returns(response);

            ManageOrderBLL ManageOrderBLL = new ManageOrderBLL(_mockManageOrderDAL.Object);
            var CuisineResponse = ManageOrderBLL.GetOrder(0);
            Assert.AreEqual(response.Orders.Count, CuisineResponse?.Orders.Count);
        }

        [TestMethod]
        public void ManageOrderBLLTest_GetCuisines_FailResponse()
        {
            var response = new OrderResponse();
            response.IsSuccessFull = false;

            _mockManageOrderDAL.Setup(x => x.GetOrder(It.IsAny<int?>())).Returns(response);

            ManageOrderBLL ManageOrderBLL = new ManageOrderBLL(_mockManageOrderDAL.Object);
            var CuisineResponse = ManageOrderBLL.GetOrder(0);
            Assert.AreEqual(response.IsSuccessFull, CuisineResponse?.IsSuccessFull);
        }

        [TestMethod]
        public void ManageOrderBLLTest_AddUpdateCuisine_SuccessResponse()
        {
            var response = new OrderResponse();
            response.IsSuccessFull = true;

            _mockManageOrderDAL.Setup(x => x.AddUpdateOrder(It.IsAny<AddUpdateOrder>())).Returns(response);

            ManageOrderBLL ManageOrderBLL = new ManageOrderBLL(_mockManageOrderDAL.Object);
            var OrderResponse = ManageOrderBLL.AddUpdateOrder(new AddUpdateOrder());
            Assert.AreEqual(response.IsSuccessFull, OrderResponse?.IsSuccessFull);
        }

        [TestMethod]
        public void ManageOrderBLLTest_AddUpdateCuisine_FailResponse()
        {
            var response = new OrderResponse();
            response.IsSuccessFull = false;

            _mockManageOrderDAL.Setup(x => x.AddUpdateOrder(It.IsAny<AddUpdateOrder>())).Returns(response);

            ManageOrderBLL ManageOrderBLL = new ManageOrderBLL(_mockManageOrderDAL.Object);
            var OrderResponse = ManageOrderBLL.AddUpdateOrder(new AddUpdateOrder());
            Assert.AreEqual(response.IsSuccessFull, OrderResponse?.IsSuccessFull);
        }
    }
}