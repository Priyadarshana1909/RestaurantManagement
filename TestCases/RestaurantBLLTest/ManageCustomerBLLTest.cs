using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestaurantBLL;
using RestaurantDAL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantBLLTest
{
    [TestClass]
    public class ManageCustomerBLLTest
    {
        private readonly Mock<IManageCustomerDAL> _mockManageCustomerDAL;

        public ManageCustomerBLLTest()
        {
            _mockManageCustomerDAL = new Mock<IManageCustomerDAL>();
        }

        [TestMethod]
        public void ManageCustomerBLLTest_GetCustomer_SuccessResponse()
        {
            var response = new CustomerResponse();
            response.IsSuccessFull = true;

            _mockManageCustomerDAL.Setup(x => x.GetCustomers(It.IsAny<int?>())).Returns(response);

            ManageCustomerBLL ManageCustomerBLL = new ManageCustomerBLL(_mockManageCustomerDAL.Object);
            var CustomerResponse = ManageCustomerBLL.GetCustomers(0);
            Assert.AreEqual(CustomerResponse.IsSuccessFull, response.IsSuccessFull);
            Assert.IsTrue(CustomerResponse.IsSuccessFull);
        }

        [TestMethod]
        public void ManageCuisineBLLTest_GetCuisines_FailResponse()
        {
            var response = new CustomerResponse();
            response.IsSuccessFull = false;

            _mockManageCustomerDAL.Setup(x => x.GetCustomers(It.IsAny<int?>())).Returns(response);

            ManageCustomerBLL ManageCustomerBLL = new ManageCustomerBLL(_mockManageCustomerDAL.Object);
            var CustomerResponse = ManageCustomerBLL.GetCustomers(0);
            Assert.AreEqual(response.IsSuccessFull, CustomerResponse?.IsSuccessFull);
        }

        [TestMethod]
        public void ManageCuisineBLL_AddUpdateCustomer_SuccessResponse()
        {
            var response = new CustomerResponse();
            response.IsSuccessFull = true;

            _mockManageCustomerDAL.Setup(x => x.AddUpdateCustomer(It.IsAny<AddUpdateCustomer>())).Returns(response);

            ManageCustomerBLL ManageCuisineBLL = new ManageCustomerBLL(_mockManageCustomerDAL.Object);
            var CustomerResponse = ManageCuisineBLL.AddUpdateCustomer(new AddUpdateCustomer());
            Assert.AreEqual(response.IsSuccessFull, CustomerResponse?.IsSuccessFull);
        }

        [TestMethod]
        public void ManageCuisineBLL_AddUpdateCuisine_FailResponse()
        {
            var response = new CustomerResponse();
            response.IsSuccessFull = false;

            _mockManageCustomerDAL.Setup(x => x.AddUpdateCustomer(It.IsAny<AddUpdateCustomer>())).Returns(response);

            ManageCustomerBLL ManageCuisineBLL = new ManageCustomerBLL(_mockManageCustomerDAL.Object);
            var CustomerResponse = ManageCuisineBLL.AddUpdateCustomer(new AddUpdateCustomer());
            Assert.AreEqual(response.IsSuccessFull, CustomerResponse?.IsSuccessFull);
        }
    }
}