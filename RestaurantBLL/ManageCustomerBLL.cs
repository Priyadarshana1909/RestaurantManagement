using RestaurantBLL.Interface;
using RestaurantDAL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantBLL
{
    public class ManageCustomerBLL : IManageCustomerBLL
    {
        private readonly IManageCustomerDAL _manageCustomerDAL;

        public ManageCustomerBLL(IManageCustomerDAL manageCustomerDAL)
        {
            _manageCustomerDAL = manageCustomerDAL;
        }

        public CustomerResponse GetCustomers(int? CustomerId)
        {
            return _manageCustomerDAL.GetCustomers(CustomerId);
        }

        public BaseResponse AddUpdateCustomer(AddUpdateCustomer AddUpdateCustomer)
        {
            return _manageCustomerDAL.AddUpdateCustomer(AddUpdateCustomer);
        }

      
    }
}
