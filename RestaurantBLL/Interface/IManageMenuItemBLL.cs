using RestaurantDTO.Response;

namespace RestaurantBLL.Interface
{
    public interface IManageMenuItemBLL
    {
        MenuItemResponse GetMenuItems(int RestaurantId);
    }
}
