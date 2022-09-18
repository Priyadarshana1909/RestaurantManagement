using RestaurantDTO.Response;

namespace RestaurantBLL.Interface
{
    public interface IManageDiningTableBLL
    {
        DiningTableResponse GetDiningTablesFromRestaurantId(int RestaurantId);
    }
}
