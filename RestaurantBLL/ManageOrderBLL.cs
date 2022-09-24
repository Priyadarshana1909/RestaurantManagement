using RestaurantBLL.Interface;
using RestaurantDAL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantBLL
{
    /// <summary>
    /// Manage order bll
    /// </summary>
    public class ManageOrderBLL : IManageOrderBLL
    {
        private IManageOrderDAL _manageOrderDAL;

        public ManageOrderBLL(IManageOrderDAL manageOrderDAL)
        {
            _manageOrderDAL = manageOrderDAL;
        }

        /// <summary>
        /// Get order details
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public OrderResponse GetOrder(int? OrderId)
        {
            return _manageOrderDAL.GetOrder(OrderId);
        }

        /// <summary>
        /// Add / update / delete order
        /// </summary>
        /// <param name="AddUpdateOrder"></param>
        /// <returns></returns>
        public BaseResponse AddUpdateOrder(AddUpdateOrder AddUpdateOrder)
        {
            return _manageOrderDAL.AddUpdateOrder(AddUpdateOrder);
        }
    }
}
