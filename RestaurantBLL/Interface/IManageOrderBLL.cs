using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantBLL.Interface
{
    public interface IManageOrderBLL
    {
        OrderResponse GetOrder(int? OrderId);

        BaseResponse AddUpdateOrder(AddUpdateOrder AddUpdateOrder);
    }
}
