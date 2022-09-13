using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantBLL.Interface
{
    public interface IManageCuisineBLL
    {
        CuisineResponse GetCuisines(int? CuisineId);

        BaseResponse AddUpdateCuisine(AddUpdateCuisine AddUpdateCuisine);
    }
}
