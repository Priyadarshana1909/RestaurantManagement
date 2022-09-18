using Microsoft.AspNetCore.Mvc;
using RestaurantBLL.Interface;
using RestaurantDTO.Response;

namespace RestaurantWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DiningTableController : Controller
    {
        private readonly IManageDiningTableBLL _manageDiningTableBLL;

        public DiningTableController(IManageDiningTableBLL manageDiningTableBLL)
        {
            _manageDiningTableBLL = manageDiningTableBLL;
        }

        [HttpGet]
        [Route("GetDiningTable/{RestaurantId}")]
        public async Task<ActionResult<DiningTableResponse>> Get(int RestaurantId = 0)
        {
            var response = _manageDiningTableBLL.GetDiningTablesFromRestaurantId(RestaurantId);
            return response;
        }
    }
}
