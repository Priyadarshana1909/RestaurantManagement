using Microsoft.AspNetCore.Mvc;
using RestaurantBLL.Interface;
using RestaurantDTO.Response;

namespace RestaurantWebAPI.Controllers
{
    /// <summary>
    /// Menu Item controller - to get menu item 
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class MenuItemController : Controller
    {
        private readonly IManageMenuItemBLL _manageMenuItemBLL;

        /// <summary>
        /// Constructor for menu item controller
        /// </summary>
        /// <param name="manageMenuItemBLL"></param>
        public MenuItemController(IManageMenuItemBLL manageMenuItemBLL)
        {
            _manageMenuItemBLL = manageMenuItemBLL;
        }

        #region "Get Menu Item"
        /// <summary>
        /// Get menu item details
        /// it will give respective menu item details
        /// </summary>
        /// <param name="RestaurantId"></param>
        /// <returns>MenuItemResponse</returns>
        [HttpGet]
        [Route("GetMenuItem/{RestaurantId}")]
        public async Task<ActionResult<MenuItemResponse>> Get(int RestaurantId = 0)
        {
            var response = _manageMenuItemBLL.GetMenuItems(RestaurantId);
            return response;
        }
        #endregion

        #region "Get Menu Item By MenuItemId"
        /// <summary>
        /// Get menu item details
        /// it will give respective menu item by MenuItemId details
        /// </summary>
        /// <param name="MenuItemID"></param>
        /// <returns>MenuItemResponse</returns>
        [HttpGet]
        [Route("GetMenuItemPrice/{MenuItemID}")]
        public async Task<ActionResult<MenuItemResponse>> GetMenuItem(int MenuItemID = 0)
        {
            var response = _manageMenuItemBLL.GetMenuItemPrice(MenuItemID);
            return response;
        }
        #endregion
    }
}
