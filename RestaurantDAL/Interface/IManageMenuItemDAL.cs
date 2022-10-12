using RestaurantDTO.Response;

namespace RestaurantDAL.Interface
{
    public interface IManageMenuItemDAL
    {
        MenuItemResponse GetMenuItemFromRestaurantId(int RestaurantId);
        MenuItemResponse GetMenuItemPrice(int? MenuItemId);
    }
}
