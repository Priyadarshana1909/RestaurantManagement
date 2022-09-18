using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantDAL.Interface
{
    public interface IManageBillDAL
    {
        BillResponse GetBill(int? BillId);
        BaseResponse AddUpdateBill(AddUpdateBill addUpdateBill);
    }
}
