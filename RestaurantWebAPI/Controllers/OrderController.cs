using Microsoft.AspNetCore.Mvc;
using RestaurantBLL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantWebAPI.Controllers
{
    /// <summary>
    /// Order controller - to get order and add update order
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class OrderController : Controller
    {
        private readonly IManageOrderBLL _manageOrderBLL;

        /// <summary>
        /// Constructor for order controller
        /// </summary>
        /// <param name="manageOrderBLL"></param>
        public OrderController(IManageOrderBLL manageOrderBLL)
        {
            _manageOrderBLL = manageOrderBLL;
        }

        #region Get Order Details
        /// <summary>
        /// Get Order details
        /// Pass 0 to get all order details otherwise 
        /// it will give respective order details
        /// In case of wrong order id - it will give IsSuccessFull = false in response
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns>OrderResponse</returns>
        [HttpGet]
        [Route("GetOrder/{OrderId}")]
        public async Task<ActionResult<OrderResponse>> GetOrder(int OrderId = 0)
        {
            var response = _manageOrderBLL.GetOrder(OrderId > 0 ? OrderId : null);
            return response;
        }
        #endregion

        #region Add / Update / Delete Order
        /// <summary>
        /// To Add / update / delete Order
        /// To delete order pass IsDelete flag to one in request
        /// </summary>
        /// <param name="AddUpdateOrder"></param>
        /// <returns>BaseResponse</returns>
        [HttpPost]
        [Route("AddUpdateOrder")]
        public async Task<ActionResult<BaseResponse>> AddUpdateOrder(AddUpdateOrder AddUpdateOrder)
        {
            var response = _manageOrderBLL.AddUpdateOrder(AddUpdateOrder);
            return response;
        }
        #endregion
    }
}
