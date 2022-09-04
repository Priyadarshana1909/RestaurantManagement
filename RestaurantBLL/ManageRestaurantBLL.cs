using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestaurantBLL.Interface;
using RestaurantDAL.Interface;
using RestaurantDTO.Response;

namespace RestaurantBLL
{
    public class ManageRestaurantBLL : IManageRestaurantBLL
    {
        private IManageRestaurantDAL _manageRestaurantDAL;

        public ManageRestaurantBLL(IManageRestaurantDAL manageRestaurantDAL)
        {
            _manageRestaurantDAL = manageRestaurantDAL;
        }

        public RestaurantResponse GetRestaurants(int? RestaurantId)
        {
            return _manageRestaurantDAL.GetRestaurants(RestaurantId);
        }
    }
}
