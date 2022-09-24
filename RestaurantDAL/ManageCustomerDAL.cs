using Microsoft.Data.SqlClient;
using RestaurantDAL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;
using System.Data;

namespace RestaurantDAL
{
    /// <summary>
    /// Mange customer dal
    /// </summary>
    public class ManageCustomerDAL : IManageCustomerDAL
    {
        private static string ConnectionString = Common.GetConnectionString();

        /// <summary>
        /// Get customer details based on customer id
        /// </summary>
        /// <param name="CustomerID">CustomerID</param>
        /// <returns></returns>
        public CustomerResponse GetCustomers(int? CustomerID)
        {
            var response = new CustomerResponse { IsSuccessFull = false };
            try
            {
               
                SqlParameter[] parameters2 = new SqlParameter[1];

                parameters2[0] = new SqlParameter("@CustomerID", CustomerID);
               
                var ds2 = SqlHelper.ExecuteDataset(ConnectionString, "USP_GetCustomer", parameters2);


                if (ds2?.Tables != null && ds2.Tables.Count > 0)
                {
                    var Customers = DataAccessHelper.ConvertToList<Customer>(ds2.Tables[0]);
                    response.IsSuccessFull = true;
                    response.Customers = Customers;
                }
                else
                {
                    response.ErrorMessage = "Invalid customer id";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccessFull = false;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// Add / update / delete customer
        /// </summary>
        /// <param name="AddUpdateCustomer">AddUpdateCustomer</param>
        /// <returns></returns>
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
