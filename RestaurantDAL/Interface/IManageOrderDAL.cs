using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantDAL.Interface
{
    public interface IManageOrderDAL
    {
        OrderResponse GetOrder(int? OrderId);

        BaseResponse AddUpdateOrder(AddUpdateOrder AddUpdateOrder);
    }
}
