using RestaurantDTO.Response;

namespace RestaurantBLL.Interface
{
    public interface IManageCuisineBLL
    {
        CuisineResponse GetCuisines(int? CuisineId);
    }
}
