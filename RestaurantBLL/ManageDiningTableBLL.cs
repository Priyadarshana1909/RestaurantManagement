using RestaurantBLL.Interface;
using RestaurantDAL.Interface;
using RestaurantDTO.Response;

namespace RestaurantBLL
{
    public class ManageDiningTableBLL : IManageDiningTableBLL
    {

        private readonly IManageDiningTableDAL _manageDiningTableDAL;

        public ManageDiningTableBLL(IManageDiningTableDAL manageDiningTableDAL)
        {
            _manageDiningTableDAL = manageDiningTableDAL;
        }

        public DiningTableResponse GetDiningTablesFromRestaurantId(int RestaurantId)
        {
            return _manageDiningTableDAL.GetDiningTablesFromRestaurantId(RestaurantId);
        }
    }
}
