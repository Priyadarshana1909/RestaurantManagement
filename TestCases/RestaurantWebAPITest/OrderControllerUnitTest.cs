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
    public class OrderControllerUnitTest
    {
        private readonly Mock<IManageOrderBLL> _mockManageOrderBll;

        public OrderControllerUnitTest()
        {
            _mockManageOrderBll = new Mock<IManageOrderBLL>();  
        }

        [TestMethod]
        public void OrderControllerUnitTest_GetOrder_SuccessResponse()
        {
            var response = new OrderResponse();
            response.IsSuccessFull = true;
            response.Orders = new List<Order>() {
                new Order{ 
                    OrderID = 1,
                    OrderAmount = 10
                }
            };
            _mockManageOrderBll.Setup(x => x.GetOrder(It.IsAny<int?>())).Returns(response);

            OrderController orderController = new OrderController(_mockManageOrderBll.Object);
            var CuisineResponse = orderController.GetOrder(0).Result;
            Assert.AreEqual(response.Orders.Count, CuisineResponse?.Value?.Orders.Count);
        }

        [TestMethod]
        public void CuisineControllerUnitTest_GetCuisine_FailResponse()
        {
            var response = new OrderResponse();
            response.IsSuccessFull = false;

            _mockManageOrderBll.Setup(x => x.GetOrder(It.IsAny<int?>())).Returns(response);

            OrderController orderController = new OrderController(_mockManageOrderBll.Object);
            var CuisineResponse = orderController.GetOrder(0).Result;
            Assert.AreEqual(response.IsSuccessFull, CuisineResponse?.Value?.IsSuccessFull);
        }

        [TestMethod]
        public void CuisineControllerUnitTest_AddUpdateCuisine_SuccessResponse()
        {
            var response = new BaseResponse();
            response.IsSuccessFull = true;

            _mockManageOrderBll.Setup(x => x.AddUpdateOrder(It.IsAny<AddUpdateOrder>())).Returns(response);

            OrderController OrderController = new OrderController(_mockManageOrderBll.Object);
            var OrderResponse = OrderController.AddUpdateOrder(new AddUpdateOrder()).Result;
            Assert.AreEqual(response.IsSuccessFull, OrderResponse?.Value?.IsSuccessFull);
        }

        [TestMethod]
        public void CuisineControllerUnitTest_AddUpdateCuisine_FailResponse()
        {
            var response = new BaseResponse();
            response.IsSuccessFull = false;

            _mockManageOrderBll.Setup(x => x.AddUpdateOrder(It.IsAny<AddUpdateOrder>())).Returns(response);

            OrderController orderController = new OrderController(_mockManageOrderBll.Object);
            var OrderResponse = orderController.AddUpdateOrder(new AddUpdateOrder()).Result;
            Assert.AreEqual(response.IsSuccessFull, OrderResponse?.Value?.IsSuccessFull);
        }
    }
}