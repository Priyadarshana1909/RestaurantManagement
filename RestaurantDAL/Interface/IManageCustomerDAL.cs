using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantDAL.Interface
{
    public interface IManageCustomerDAL
    {
        CustomerResponse GetCustomers(int? CustomerID);

        BaseResponse AddUpdateCustomer(AddUpdateCustomer AddUpdateCustomer);
    }
}
