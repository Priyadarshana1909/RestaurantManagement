using RestaurantBLL.Interface;
using RestaurantDAL.Interface;
using RestaurantDTO.Response;

namespace RestaurantBLL
{
    public class ManageBillBLL : IManageBillBLL
    {
        private IManageBillDAL _manageBillDAL;

        public ManageBillBLL(IManageBillDAL manageBillDAL)
        {
            _manageBillDAL = manageBillDAL;
        }
        public BillResponse GetBill(int? BillId)
        {
            return _manageBillDAL.GetBill(BillId);
        }
    }
}
