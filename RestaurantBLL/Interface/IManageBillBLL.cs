using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantBLL.Interface
{
    public interface IManageBillBLL
    {
        BillResponse GetBill(int? BillId);
        BaseResponse AddUpdateBill(AddUpdateBill addUpdateBill);
    }
}
