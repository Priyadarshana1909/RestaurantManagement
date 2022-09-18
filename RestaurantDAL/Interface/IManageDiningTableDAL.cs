using RestaurantDTO.Response;

namespace RestaurantDAL.Interface
{
    public interface IManageDiningTableDAL
    {
        DiningTableResponse GetDiningTablesFromRestaurantId(int RestaurantId);
    }
}
