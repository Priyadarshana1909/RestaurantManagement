using RestaurantDTO.Response;

namespace RestaurantBLL.Interface
{
    public interface IManageBillBLL
    {
        BillResponse GetBill(int? BillId);
    }
}
