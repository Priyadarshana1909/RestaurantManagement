using RestaurantBLL.Interface;
using RestaurantDAL.Interface;
using RestaurantDTO.Response;

namespace RestaurantBLL
{
    /// <summary>
    /// Manage restaurant bll
    /// </summary>
    public class ManageRestaurantBLL : IManageRestaurantBLL
    {
        private IManageRestaurantDAL _manageRestaurantDAL;

        public ManageRestaurantBLL(IManageRestaurantDAL manageRestaurantDAL)
        {
            _manageRestaurantDAL = manageRestaurantDAL;
        }

        /// <summary>
        /// Get restaurants details based on restaurant id
        /// </summary>
        /// <param name="RestaurantId"></param>
        /// <returns></returns>
        public RestaurantResponse GetRestaurants(int? RestaurantId)
        {
            return _manageRestaurantDAL.GetRestaurants(RestaurantId);
        }
    }
}
