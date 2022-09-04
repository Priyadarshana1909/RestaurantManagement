
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

       

        [HttpGet(Name = "GetRestaurant")]
        public async Task<ActionResult<RestaurantResponse>> Get()
        {
            var response = _manageRestaurantBLL.GetRestaurants(null);

            return response;
        }
    }
}
