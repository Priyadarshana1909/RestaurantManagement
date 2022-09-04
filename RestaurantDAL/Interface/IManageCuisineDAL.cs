using RestaurantDTO.Response;

namespace RestaurantDAL.Interface
{
    public interface IManageCuisineDAL
    {
        CuisineResponse GetCuisines(int? CuisineId);
    }
}
