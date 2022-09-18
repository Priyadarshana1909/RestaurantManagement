using RestaurantBLL.Interface;
using RestaurantDAL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantBLL
{
    public class ManageOrderBLL : IManageOrderBLL
    {
        private IManageOrderDAL _manageOrderDAL;

        public ManageOrderBLL(IManageOrderDAL manageOrderDAL)
        {
            _manageOrderDAL = manageOrderDAL;
        }
        public OrderResponse GetOrder(int? OrderId)
        {
            return _manageOrderDAL.GetOrder(OrderId);
        }

        public BaseResponse AddUpdateOrder(AddUpdateOrder AddUpdateOrder)
        {
            return _manageOrderDAL.AddUpdateOrder(AddUpdateOrder);
        }
    }
}
