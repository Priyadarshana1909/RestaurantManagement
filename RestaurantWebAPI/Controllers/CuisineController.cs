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

        [HttpGet]
        [Route("GetCuisine/{CuisineId}")]
        public async Task<ActionResult<CuisineResponse>> Get(int CuisineId = 0)
        {
            var response = _manageCuisineBLL.GetCuisines(CuisineId > 0? CuisineId : null);
            return response;
        }
    }
}
