using Microsoft.AspNetCore.Mvc;
using RestaurantBLL.Interface;
using RestaurantDTO.Response;

namespace RestaurantWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CuisineController : Controller
    {
        private readonly IManageCuisineBLL _manageCuisineBLL;

        public CuisineController(IManageCuisineBLL manageCuisineBLL)
        {
            _manageCuisineBLL = manageCuisineBLL;
        }

        [HttpGet(Name = "GetCuisine")]
        public async Task<ActionResult<CuisineResponse>> Get()
        {
            var response = _manageCuisineBLL.GetCuisines(null);
            return response;
        }
    }
}
