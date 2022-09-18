using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestaurantDTO.Response;
using RestaurantWebApplication.Controllers;
using RestaurantWebApplication.Services.Interface;
using System.Collections.Generic;
using System.Net.Http;

namespace RestaurantWebApplicationTest
{
    [TestClass]
    public class CuisineControllerUnitTest
    {
        private readonly Mock<IAPIService> _mockAPIService;
        private readonly Mock<ILogger<CuisineController>> _mockLogger;

        public CuisineControllerUnitTest()
        {
            _mockAPIService = new Mock<IAPIService>();
            _mockLogger = new Mock<ILogger<CuisineController>>();   
        }

        [TestMethod]
        public void CuisineController_Index_SuccessResponse()
        {
            var response = new CuisineResponse();
            response.IsSuccessFull = true;
            response.Cuisines = new List<Cuisine>() {
                new Cuisine{
                    CuisineID = 1,
                    CuisineName = ""
                }
            };
            _mockAPIService.Setup(x => x.ExecuteRequest<CuisineResponse>(It.IsAny<string>(), It.IsAny<HttpMethod>(),null)).ReturnsAsync(response);

            CuisineController CuisineController = new CuisineController(_mockLogger.Object, _mockAPIService.Object);
            var CuisineResponse = CuisineController.Index().Result;
            Assert.AreEqual(response.Cuisines.Count, ((List<RestaurantDTO.Response.Cuisine>)((Microsoft.AspNetCore.Mvc.ViewResult)CuisineResponse).Model).Count);
        }

        [TestMethod]
        public void CuisineController_CreateHttpGet_SuccessResponse()
        {
            var response = new RestaurantResponse();
            response.IsSuccessFull = true;
            response.Restaurants = new List<Restaurant>() {
                new Restaurant{
                    RestaurantID = 1,
                    RestaurantName = ""
                }
            };
            _mockAPIService.Setup(x => x.ExecuteRequest<RestaurantResponse>(It.IsAny<string>(), It.IsAny<HttpMethod>(), null)).ReturnsAsync(response);

            CuisineController CuisineController = new CuisineController(_mockLogger.Object, _mockAPIService.Object);
            var CuisineCreateResponse = CuisineController.Create().Result;
            Assert.AreEqual(response.Restaurants.Count, ((RestaurantDTO.Request.AddUpdateCuisine)((Microsoft.AspNetCore.Mvc.ViewResult)CuisineCreateResponse).Model).Restaurants.Count);
        }

        [TestMethod]
        public void CuisineController_CreateHttpGet_FailResponse()
        {
            var response = new RestaurantResponse();
            response.IsSuccessFull = false;
          
            _mockAPIService.Setup(x => x.ExecuteRequest<RestaurantResponse>(It.IsAny<string>(), It.IsAny<HttpMethod>(), null)).ReturnsAsync(response);

            CuisineController CuisineController = new CuisineController(_mockLogger.Object, _mockAPIService.Object);
            var CuisineCreateResponse = CuisineController.Create().Result;
            Assert.AreEqual(0, ((RestaurantDTO.Request.AddUpdateCuisine)((Microsoft.AspNetCore.Mvc.ViewResult)CuisineCreateResponse).Model).Restaurants.Count);
        }
    }
}