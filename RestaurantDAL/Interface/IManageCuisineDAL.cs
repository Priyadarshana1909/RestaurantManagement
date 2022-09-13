using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantDAL.Interface
{
    public interface IManageCuisineDAL
    {
        CuisineResponse GetCuisines(int? CuisineId);

        BaseResponse AddUpdateCuisine(AddUpdateCuisine addUpdateCuisine);
    }
}
