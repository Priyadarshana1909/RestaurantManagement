using Microsoft.AspNetCore.Mvc;
using RestaurantBLL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : Controller
    {
        private readonly IManageOrderBLL _manageOrderBLL;

        public OrderController(IManageOrderBLL manageOrderBLL)
        {
            _manageOrderBLL = manageOrderBLL;
        }

        [HttpGet]
        [Route("GetOrder/{OrderId}")]
        public async Task<ActionResult<OrderResponse>> GetOrder(int OrderId = 0)
        {
            var response = _manageOrderBLL.GetOrder(OrderId > 0 ? OrderId : null);
            return response;
        }

        [HttpPost]
        [Route("AddUpdateOrder")]
        public async Task<ActionResult<BaseResponse>> AddUpdateOrder(AddUpdateOrder AddUpdateOrder)
        {
            var response = _manageOrderBLL.AddUpdateOrder(AddUpdateOrder);
            return response;
        }
    }
}
