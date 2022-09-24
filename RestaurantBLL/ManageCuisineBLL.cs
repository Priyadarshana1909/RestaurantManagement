using RestaurantBLL.Interface;
using RestaurantDAL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantBLL
{
    /// <summary>
    /// Manage cuisine bll
    /// </summary>
    public class ManageCuisineBLL : IManageCuisineBLL
    {
        private readonly IManageCuisineDAL _manageCuisineDAL;

        public ManageCuisineBLL(IManageCuisineDAL manageCuisineDAL)
        {
            _manageCuisineDAL = manageCuisineDAL;
        }

        /// <summary>
        /// Get cuisines
        /// </summary>
        /// <param name="CuisineId"></param>
        /// <returns></returns>
        public CuisineResponse GetCuisines(int? CuisineId)
        {
            return _manageCuisineDAL.GetCuisines(CuisineId);
        }

        /// <summary>
        /// Add update delete cuisine
        /// </summary>
        /// <param name="AddUpdateCuisine"></param>
        /// <returns></returns>
        public BaseResponse AddUpdateCuisine(AddUpdateCuisine AddUpdateCuisine)
        {
            return _manageCuisineDAL.AddUpdateCuisine(AddUpdateCuisine);
        }
    }
}
