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
    public class CustomerControllerUnitTest
    {
        private readonly Mock<IManageCustomerBLL> _mockManageCustomerBll;

        public CustomerControllerUnitTest()
        {
            _mockManageCustomerBll = new Mock<IManageCustomerBLL>();  
        }

        [TestMethod]
        public void CustomerControllerUnitTest_GetCustomer_SuccessResponse()
        {
            var response = new CustomerResponse();
            response.IsSuccessFull = true;
            response.Customers = new List<Customer>() {
                new Customer{ 
                    CustomerID = 1,
                    CustomerName = "CustomerName"
                }
            };
            _mockManageCustomerBll.Setup(x => x.GetCustomers(It.IsAny<int?>())).Returns(response);

            CustomerController CustomerController = new CustomerController(_mockManageCustomerBll.Object);
            var CuisineResponse = CustomerController.GetCustomers(0).Result;
            Assert.AreEqual(response.Customers.Count, CuisineResponse?.Value?.Customers.Count);
        }

        [TestMethod]
        public void CuisineControllerUnitTest_GetCuisine_FailResponse()
        {
            var response = new CustomerResponse();
            response.IsSuccessFull = false;

            _mockManageCustomerBll.Setup(x => x.GetCustomers(It.IsAny<int?>())).Returns(response);

            CustomerController CustomerController = new CustomerController(_mockManageCustomerBll.Object);
            var CuisineResponse = CustomerController.GetCustomers(0).Result;
            Assert.AreEqual(response.IsSuccessFull, CuisineResponse?.Value?.IsSuccessFull);
        }

        [TestMethod]
        public void CuisineControllerUnitTest_AddUpdateCuisine_SuccessResponse()
        {
            var response = new BaseResponse();
            response.IsSuccessFull = true;

            _mockManageCustomerBll.Setup(x => x.AddUpdateCustomer(It.IsAny<AddUpdateCustomer>())).Returns(response);

            CustomerController CustomerController = new CustomerController(_mockManageCustomerBll.Object);
            var CustomerResponse = CustomerController.AddUpdateCustomer(new AddUpdateCustomer()).Result;
            Assert.AreEqual(response.IsSuccessFull, CustomerResponse?.Value?.IsSuccessFull);
        }

        [TestMethod]
        public void CuisineControllerUnitTest_AddUpdateCuisine_FailResponse()
        {
            var response = new BaseResponse();
            response.IsSuccessFull = false;

            _mockManageCustomerBll.Setup(x => x.AddUpdateCustomer(It.IsAny<AddUpdateCustomer>())).Returns(response);

            CustomerController CustomerController = new CustomerController(_mockManageCustomerBll.Object);
            var CustomerResponse = CustomerController.AddUpdateCustomer(new AddUpdateCustomer()).Result;
            Assert.AreEqual(response.IsSuccessFull, CustomerResponse?.Value?.IsSuccessFull);
        }
    }
}