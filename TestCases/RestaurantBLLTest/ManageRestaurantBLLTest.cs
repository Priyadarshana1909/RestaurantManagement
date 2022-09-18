using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestaurantBLL;
using RestaurantDAL.Interface;
using RestaurantDTO.Response;

namespace RestaurantBLLTest
{
    [TestClass]
    public class ManageRestaurantBLLTest
    {
        private readonly Mock<IManageRestaurantDAL> _mockManageOrderDAL;

        public ManageRestaurantBLLTest()
        {
            _mockManageOrderDAL = new Mock<IManageRestaurantDAL>();
        }

        [TestMethod]
        public void ManageOrderBLLTest_GetRestaurants_SuccessResponse()
        {
            var response = new RestaurantResponse();
            response.IsSuccessFull = true;

            _mockManageOrderDAL.Setup(x => x.GetRestaurants(It.IsAny<int?>())).Returns(response);

            ManageRestaurantBLL ManageRestaurantBLL = new ManageRestaurantBLL(_mockManageOrderDAL.Object);
            var RestaurantResponse = ManageRestaurantBLL.GetRestaurants(0);
            Assert.AreEqual(response.Restaurants.Count, RestaurantResponse?.Restaurants.Count);
        }

        [TestMethod]
        public void ManageOrderBLLTest_GetRestaurants_FailResponse()
        {
            var response = new RestaurantResponse();
            response.IsSuccessFull = false;

            _mockManageOrderDAL.Setup(x => x.GetRestaurants(It.IsAny<int?>())).Returns(response);

            ManageRestaurantBLL ManageRestaurantBLL = new ManageRestaurantBLL(_mockManageOrderDAL.Object);
            var RestaurantResponse = ManageRestaurantBLL.GetRestaurants(0);
            Assert.AreEqual(response.IsSuccessFull, RestaurantResponse?.IsSuccessFull);
        }
       
    }
}