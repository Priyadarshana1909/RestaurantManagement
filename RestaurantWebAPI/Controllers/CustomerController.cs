using Microsoft.AspNetCore.Mvc;
using RestaurantBLL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : Controller
    {
        private readonly IManageCustomerBLL _manageCustomerBLL;

        public CustomerController(IManageCustomerBLL manageCustomerBLL)
        {
            _manageCustomerBLL = manageCustomerBLL;
        }

        [HttpGet]
        [Route("GetCustomer/{CustomerId}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomers(int CustomerId = 0)
        {
            var response = _manageCustomerBLL.GetCustomers(CustomerId > 0? CustomerId : null);
            return response;
        }

        [HttpPost]
        [Route("AddUpdateCustomer")]
        public async Task<ActionResult<BaseResponse>> AddUpdateCustomer(AddUpdateCustomer AddUpdateCustomer)
        {
            var response = _manageCustomerBLL.AddUpdateCustomer(AddUpdateCustomer);
            return response;
        }
    }
}
