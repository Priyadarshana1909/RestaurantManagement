using Microsoft.AspNetCore.Mvc;
using RestaurantBLL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantWebAPI.Controllers
{
    /// <summary>
    /// Customer controller - to get customer and add update customer
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : Controller
    {
        private readonly IManageCustomerBLL _manageCustomerBLL;

        /// <summary>
        /// Constructor for customer controller
        /// </summary>
        /// <param name="manageCustomerBLL"></param>
        public CustomerController(IManageCustomerBLL manageCustomerBLL)
        {
            _manageCustomerBLL = manageCustomerBLL;
        }

        #region "Get Customer Details"
        /// <summary>
        /// Get Customer details
        /// Pass 0 to get all customer details otherwise 
        /// it will give respective customer details
        /// In case of wrong customer id - it will give IsSuccessFull = false in response
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns>CustomerResponse</returns>
        [HttpGet]
        [Route("GetCustomer/{CustomerId}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomers(int CustomerId = 0)
        {
            var response = _manageCustomerBLL.GetCustomers(CustomerId > 0? CustomerId : null);
            return response;
        }
        #endregion

        #region "Add Update Delete Customer"
        /// <summary>
        /// To Add / update / delete customer
        /// To delete customer pass IsDelete flag to one in request
        /// </summary>
        /// <param name="AddUpdateCustomer"></param>
        /// <returns>BaseResponse</returns>
        [HttpPost]
        [Route("AddUpdateCustomer")]
        public async Task<ActionResult<BaseResponse>> AddUpdateCustomer(AddUpdateCustomer AddUpdateCustomer)
        {
            var response = _manageCustomerBLL.AddUpdateCustomer(AddUpdateCustomer);
            return response;
        }
        #endregion
    }
}
