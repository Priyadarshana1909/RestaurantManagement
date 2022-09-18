using RestaurantBLL.Interface;
using RestaurantDAL.Interface;
using RestaurantDTO.Response;

namespace RestaurantBLL
{
    public class ManageMenuItemBLL : IManageMenuItemBLL
    {
        private IManageMenuItemDAL _manageMenuItemDAL;

        public ManageMenuItemBLL(IManageMenuItemDAL manageMenuItemDAL)
        {
            _manageMenuItemDAL = manageMenuItemDAL;
        }

        public MenuItemResponse GetMenuItems(int RestaurantId)
        {
            return _manageMenuItemDAL.GetMenuItemFromRestaurantId(RestaurantId);
        }
    }
}
