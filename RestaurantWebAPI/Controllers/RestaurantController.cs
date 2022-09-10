
using Microsoft.AspNetCore.Mvc;
using RestaurantBLL.Interface;
using RestaurantDTO.Response;

namespace RestaurantWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RestaurantController : Controller
    {
        private readonly IManageRestaurantBLL _manageRestaurantBLL;

        public RestaurantController(IManageRestaurantBLL manageRestaurantBLL)
        {
            _manageRestaurantBLL = manageRestaurantBLL;
        }

       

        [HttpGet]
        [Route("GetRestaurant/{RestaurantId}")]
        public async Task<ActionResult<RestaurantResponse>> Get(int RestaurantId = 0)
        {
            var response = _manageRestaurantBLL.GetRestaurants(RestaurantId > 0 ? RestaurantId : null);

            return response;
        }
    }
}
