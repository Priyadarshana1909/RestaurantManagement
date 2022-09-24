using RestaurantBLL.Interface;
using RestaurantDAL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantBLL
{
    /// <summary>
    /// Manage customer bll
    /// </summary>
    public class ManageCustomerBLL : IManageCustomerBLL
    {
        private readonly IManageCustomerDAL _manageCustomerDAL;

        public ManageCustomerBLL(IManageCustomerDAL manageCustomerDAL)
        {
            _manageCustomerDAL = manageCustomerDAL;
        }

        /// <summary>
        /// Get customers
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public CustomerResponse GetCustomers(int? CustomerId)
        {
            return _manageCustomerDAL.GetCustomers(CustomerId);
        }

        /// <summary>
        /// Add update delete customer
        /// </summary>
        /// <param name="AddUpdateCustomer"></param>
        /// <returns></returns>
        public BaseResponse AddUpdateCustomer(AddUpdateCustomer AddUpdateCustomer)
        {
            return _manageCustomerDAL.AddUpdateCustomer(AddUpdateCustomer);
        }
      
    }
}
