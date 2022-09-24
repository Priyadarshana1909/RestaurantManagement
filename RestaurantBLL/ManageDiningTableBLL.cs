using RestaurantBLL.Interface;
using RestaurantDAL.Interface;
using RestaurantDTO.Response;

namespace RestaurantBLL
{
    /// <summary>
    /// Manage dining table dll
    /// </summary>
    public class ManageDiningTableBLL : IManageDiningTableBLL
    {

        private readonly IManageDiningTableDAL _manageDiningTableDAL;

        public ManageDiningTableBLL(IManageDiningTableDAL manageDiningTableDAL)
        {
            _manageDiningTableDAL = manageDiningTableDAL;
        }

        /// <summary>
        /// Get dining table details restaurant id wise
        /// </summary>
        /// <param name="RestaurantId"></param>
        /// <returns></returns>
        public DiningTableResponse GetDiningTablesFromRestaurantId(int RestaurantId)
        {
            return _manageDiningTableDAL.GetDiningTablesFromRestaurantId(RestaurantId);
        }
    }
}
