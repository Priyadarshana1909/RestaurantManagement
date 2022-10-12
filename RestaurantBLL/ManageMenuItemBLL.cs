using RestaurantBLL.Interface;
using RestaurantDAL.Interface;
using RestaurantDTO.Response;

namespace RestaurantBLL
{
    /// <summary>
    /// Manage menu item bll
    /// </summary>
    public class ManageMenuItemBLL : IManageMenuItemBLL
    {
        private IManageMenuItemDAL _manageMenuItemDAL;

        public ManageMenuItemBLL(IManageMenuItemDAL manageMenuItemDAL)
        {
            _manageMenuItemDAL = manageMenuItemDAL;
        }

        /// <summary>
        /// Get menu item details - restautant id wise
        /// </summary>
        /// <param name="RestaurantId"></param>
        /// <returns></returns>
        public MenuItemResponse GetMenuItems(int RestaurantId)
        {
            return _manageMenuItemDAL.GetMenuItemFromRestaurantId(RestaurantId);
        }

        public MenuItemResponse GetMenuItemPrice(int MenuItemId)
        {
            return _manageMenuItemDAL.GetMenuItemPrice(MenuItemId);
        }
    }
}
