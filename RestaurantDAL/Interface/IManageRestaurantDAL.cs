using RestaurantDTO.Response;

namespace RestaurantDAL.Interface
{
    public interface IManageRestaurantDAL
    {
        RestaurantResponse GetRestaurants(int? RestaurantId);
    }
}
