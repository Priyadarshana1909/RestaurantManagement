using RestaurantDTO.Response;

namespace RestaurantBLL.Interface
{
    public interface IManageRestaurantBLL
    {
        RestaurantResponse GetRestaurants(int? RestaurantId);
    }
}
