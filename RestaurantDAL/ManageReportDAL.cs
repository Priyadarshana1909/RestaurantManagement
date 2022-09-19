using Microsoft.Data.SqlClient;
using RestaurantDAL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;
using System.Data;

namespace RestaurantDAL
{
    public class ManageReportDAL : IManageReportDAL
    {
        private static string ConnectionString = Common.GetConnectionString();

        public ReportResponse GetReport(int? CustomerID)
        {
            var response = new ReportResponse { IsSuccessFull = false };
            try
            {
               
                SqlParameter[] parameters2 = new SqlParameter[1];

                parameters2[0] = new SqlParameter("@CustomerID", CustomerID);
               
                var ds2 = SqlHelper.ExecuteDataset(ConnectionString, "USP_GetCustomerDynamically", parameters2);

                List<CustomerReport> customers = new();
                if (ds2 != null && ds2.Tables.Count > 0)
                    customers = DataAccessHelper.ConvertToList<CustomerReport>(ds2.Tables[0]);

                response.IsSuccessFull = true;
                response.CustomerReports = customers;
            }
            catch (Exception ex)
            {
                response.IsSuccessFull = false;
                response.ErrorMessage = ex.Message;
            }
            return response;       }

       
    }
}
