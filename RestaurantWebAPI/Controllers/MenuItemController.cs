using Microsoft.AspNetCore.Mvc;
using RestaurantBLL.Interface;
using RestaurantDTO.Response;

namespace RestaurantWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MenuItemController : Controller
    {
        private readonly IManageMenuItemBLL _manageMenuItemBLL;

        public MenuItemController(IManageMenuItemBLL manageMenuItemBLL)
        {
            _manageMenuItemBLL = manageMenuItemBLL;
        }

        [HttpGet]
        [Route("GetMenuItem/{RestaurantId}")]
        public async Task<ActionResult<MenuItemResponse>> Get(int RestaurantId = 0)
        {
            var response = _manageMenuItemBLL.GetMenuItems(RestaurantId);

            return response;
        }
    }
}
