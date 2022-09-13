using RestaurantBLL.Interface;
using RestaurantDAL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantBLL
{
    public class ManageCuisineBLL : IManageCuisineBLL
    {
        private readonly IManageCuisineDAL _manageCuisineDAL;

        public ManageCuisineBLL(IManageCuisineDAL manageCuisineDAL)
        {
            _manageCuisineDAL = manageCuisineDAL;
        }

        public CuisineResponse GetCuisines(int? CuisineId)
        {
            return _manageCuisineDAL.GetCuisines(CuisineId);
        }

        public BaseResponse AddUpdateCuisine(AddUpdateCuisine AddUpdateCuisine)
        {
            return _manageCuisineDAL.AddUpdateCuisine(AddUpdateCuisine);
        }
    }
}
