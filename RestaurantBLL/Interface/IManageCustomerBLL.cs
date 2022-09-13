using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantBLL.Interface
{
    public interface IManageCustomerBLL
    {
        CustomerResponse GetCustomers(int? CustomerId);

        BaseResponse AddUpdateCustomer(AddUpdateCustomer AddUpdateCustomer);
    }
}
