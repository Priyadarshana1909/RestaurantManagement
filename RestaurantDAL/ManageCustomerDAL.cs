using Microsoft.Data.SqlClient;
using RestaurantDAL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;
using System.Data;

namespace RestaurantDAL
{
    public class ManageCustomerDAL : IManageCustomerDAL
    {
        private static string ConnectionString = Common.GetConnectionString();
        public CustomerResponse GetCustomers(int? CustomerID)
        {
            var response = new CustomerResponse { IsSuccessFull = false };
            try
            {
               
                SqlParameter[] parameters2 = new SqlParameter[1];

                parameters2[0] = new SqlParameter("@CustomerID", CustomerID);
               
                var ds2 = SqlHelper.ExecuteDataset(ConnectionString, "USP_GetCustomer", parameters2);

                List<Customer> Customers = new();
                if (ds2 != null && ds2.Tables.Count > 0)
                    Customers = DataAccessHelper.ConvertToList<Customer>(ds2.Tables[0]);

                response.IsSuccessFull = true;
                response.Customers = Customers;
            }
            catch (Exception ex)
            {
                response.IsSuccessFull = false;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }

        public BaseResponse AddUpdateCustomer(AddUpdateCustomer AddUpdateCustomer)
        {
            var response = new BaseResponse();
            try
            {

                SqlParameter[] parameters2 = new SqlParameter[6];
                var newlyaddedValue = 0;
                parameters2[0] = new SqlParameter("@CustomerID", AddUpdateCustomer.CustomerId);
                parameters2[1] = new SqlParameter("@RestaurantID", AddUpdateCustomer.RestaurantId);
                parameters2[2] = new SqlParameter("@CustomerName", AddUpdateCustomer.CustomerName);
                parameters2[3] = new SqlParameter("@MobileNo", AddUpdateCustomer.MobileNo);
                parameters2[4] = new SqlParameter("@IsDelete", AddUpdateCustomer.IsDelete);
                parameters2[5] = new SqlParameter("@OutputID", newlyaddedValue)
                {
                    Direction = ParameterDirection.Output
                 
                };

                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "USP_Customer", parameters2);
                response.IsSuccessFull = true;
            }
            catch (Exception ex)
            {
                response.IsSuccessFull = false;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }
    }
}
