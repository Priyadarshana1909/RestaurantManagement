using RestaurantBLL.Interface;
using RestaurantDAL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantBLL
{
    /// <summary>
    /// Bll to manage bill
    /// </summary>
    public class ManageBillBLL : IManageBillBLL
    {
        private IManageBillDAL _manageBillDAL;

        public ManageBillBLL(IManageBillDAL manageBillDAL)
        {
            _manageBillDAL = manageBillDAL;
        }

        /// <summary>
        /// Get Bill
        /// </summary>
        /// <param name="BillId"></param>
        /// <returns></returns>
        public BillResponse GetBill(int? BillId)
        {
            return _manageBillDAL.GetBill(BillId);
        }

        /// <summary>
        /// Add update delete bill
        /// </summary>
        /// <param name="AddUpdateBill"></param>
        /// <returns></returns>
        public BaseResponse AddUpdateBill(AddUpdateBill AddUpdateBill)
        {
            return _manageBillDAL.AddUpdateBill(AddUpdateBill);
        }
    }
}
