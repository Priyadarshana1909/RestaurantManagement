using Microsoft.AspNetCore.Mvc;
using RestaurantBLL.Interface;
using RestaurantDTO.Response;

namespace RestaurantWebAPI.Controllers
{
    /// <summary>
    /// Restaurant controller - to get restaurant details
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class RestaurantController : Controller
    {
        private readonly IManageRestaurantBLL _manageRestaurantBLL;

        /// <summary>
        /// Constructor for getting restaurant 
        /// </summary>
        /// <param name="manageRestaurantBLL"></param>
        public RestaurantController(IManageRestaurantBLL manageRestaurantBLL)
        {
            _manageRestaurantBLL = manageRestaurantBLL;
        }

        #region "Get Restaurant Details
        /// <summary>
        /// Get restaurant details
        /// it will give restaurant details for respective restaurant id
        /// </summary>
        /// <param name="RestaurantId"></param>
        /// <returns>RestaurantResponse</returns>
        [HttpGet]
        [Route("GetRestaurant/{RestaurantId}")]
        public async Task<ActionResult<RestaurantResponse>> Get(int RestaurantId = 0)
        {
            var response = _manageRestaurantBLL.GetRestaurants(RestaurantId > 0 ? RestaurantId : null);
            return response;
        }
        #endregion
    }
}
