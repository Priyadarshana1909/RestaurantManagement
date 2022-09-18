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
    public class CuisineControllerUnitTest
    {
        private readonly Mock<IManageCuisineBLL> _mockManageCuisineBll;

        public CuisineControllerUnitTest()
        {
            _mockManageCuisineBll = new Mock<IManageCuisineBLL>();  
        }

        [TestMethod]
        public void CuisineControllerUnitTest_GetCuisine_SuccessResponse()
        {
            var response = new CuisineResponse();
            response.IsSuccessFull = true;
            response.Cuisines = new List<Cuisine>() {
                new Cuisine{ 
                    CuisineID = 1,
                    CuisineName = "CuisineName"
                }
            };
            _mockManageCuisineBll.Setup(x => x.GetCuisines(It.IsAny<int?>())).Returns(response);

            CuisineController cuisineController = new CuisineController(_mockManageCuisineBll.Object);
            var CuisineResponse = cuisineController.Get(0).Result;
            Assert.AreEqual(response.Cuisines.Count, CuisineResponse?.Value?.Cuisines.Count);
        }

        [TestMethod]
        public void CuisineControllerUnitTest_GetCuisine_FailResponse()
        {
            var response = new CuisineResponse();
            response.IsSuccessFull = false;
            
            _mockManageCuisineBll.Setup(x => x.GetCuisines(It.IsAny<int?>())).Returns(response);

            CuisineController cuisineController = new CuisineController(_mockManageCuisineBll.Object);
            var CuisineResponse = cuisineController.Get(0).Result;
            Assert.AreEqual(response.IsSuccessFull, CuisineResponse?.Value?.IsSuccessFull);
        }

        [TestMethod]
        public void CuisineControllerUnitTest_AddUpdateCuisine_SuccessResponse()
        {
            var response = new BaseResponse();
            response.IsSuccessFull = true;
            
           _mockManageCuisineBll.Setup(x => x.AddUpdateCuisine(It.IsAny<AddUpdateCuisine>())).Returns(response);

            CuisineController cuisineController = new CuisineController(_mockManageCuisineBll.Object);
            var CuisineResponse = cuisineController.AddUpdateCuisine(new AddUpdateCuisine()).Result;
            Assert.AreEqual(response.IsSuccessFull, CuisineResponse?.Value?.IsSuccessFull);
        }

        [TestMethod]
        public void CuisineControllerUnitTest_AddUpdateCuisine_FailResponse()
        {
            var response = new BaseResponse();
            response.IsSuccessFull = false;

            _mockManageCuisineBll.Setup(x => x.AddUpdateCuisine(It.IsAny<AddUpdateCuisine>())).Returns(response);

            CuisineController cuisineController = new CuisineController(_mockManageCuisineBll.Object);
            var CuisineResponse = cuisineController.AddUpdateCuisine(new AddUpdateCuisine()).Result;
            Assert.AreEqual(response.IsSuccessFull, CuisineResponse?.Value?.IsSuccessFull);
        }
    }
}