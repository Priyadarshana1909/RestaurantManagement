using RestaurantDTO.Response;

namespace RestaurantDAL.Interface
{
    public interface IManageBillDAL
    {
        BillResponse GetBill(int? BillId);
    }
}
