using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestaurantBLL;
using RestaurantDAL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantBLLTest
{
    [TestClass]
    public class ManageCuisineBLLTest
    {
        private readonly Mock<IManageCuisineDAL> _mockManageCuisineDAL;

        public ManageCuisineBLLTest()
        {
            _mockManageCuisineDAL = new Mock<IManageCuisineDAL>();
        }

        [TestMethod]
        public void ManageCuisineBLLTest_GetCuisines_SuccessResponse()
        {
            var response = new CuisineResponse();
            response.IsSuccessFull = true;

            _mockManageCuisineDAL.Setup(x => x.GetCuisines(It.IsAny<int?>())).Returns(response);

            ManageCuisineBLL ManageCuisineBLL = new ManageCuisineBLL(_mockManageCuisineDAL.Object);
            var CuisineResponse = ManageCuisineBLL.GetCuisines(0);
            Assert.AreEqual(response.Cuisines.Count, CuisineResponse?.Cuisines.Count);
        }

        [TestMethod]
        public void ManageCuisineBLLTest_GetCuisines_FailResponse()
        {
            var response = new CuisineResponse();
            response.IsSuccessFull = false;

            _mockManageCuisineDAL.Setup(x => x.GetCuisines(It.IsAny<int?>())).Returns(response);

            ManageCuisineBLL ManageCuisineBLL = new ManageCuisineBLL(_mockManageCuisineDAL.Object);
            var CuisineResponse = ManageCuisineBLL.GetCuisines(0);
            Assert.AreEqual(response.IsSuccessFull, CuisineResponse?.IsSuccessFull);
        }

        [TestMethod]
        public void ManageCuisineBLL_AddUpdateCuisine_SuccessResponse()
        {
            var response = new CuisineResponse();
            response.IsSuccessFull = true;

            _mockManageCuisineDAL.Setup(x => x.AddUpdateCuisine(It.IsAny<AddUpdateCuisine>())).Returns(response);

            ManageCuisineBLL ManageCuisineBLL = new ManageCuisineBLL(_mockManageCuisineDAL.Object);
            var CuisineResponse = ManageCuisineBLL.AddUpdateCuisine(new AddUpdateCuisine());
            Assert.AreEqual(response.IsSuccessFull, CuisineResponse?.IsSuccessFull);
        }

        [TestMethod]
        public void ManageCuisineBLL_AddUpdateCuisine_FailResponse()
        {
            var response = new CuisineResponse();
            response.IsSuccessFull = false;

            _mockManageCuisineDAL.Setup(x => x.AddUpdateCuisine(It.IsAny<AddUpdateCuisine>())).Returns(response);

            ManageCuisineBLL ManageCuisineBLL = new ManageCuisineBLL(_mockManageCuisineDAL.Object);
            var CuisineResponse = ManageCuisineBLL.AddUpdateCuisine(new AddUpdateCuisine());
            Assert.AreEqual(response.IsSuccessFull, CuisineResponse?.IsSuccessFull);
        }
    }
}