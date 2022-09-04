using RestaurantBLL.Interface;
using RestaurantDAL.Interface;
using RestaurantDTO.Response;

namespace RestaurantBLL
{
    public class ManageCuisineBLL : IManageCuisineBLL
    {
        private IManageCuisineDAL _manageCuisineDAL;

        public ManageCuisineBLL(IManageCuisineDAL manageCuisineDAL)
        {
            _manageCuisineDAL = manageCuisineDAL;
        }

        public CuisineResponse GetCuisines(int? CuisineId)
        {
            return _manageCuisineDAL.GetCuisines(CuisineId);
        }
    }
}
