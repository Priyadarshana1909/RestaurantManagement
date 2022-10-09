using Microsoft.Data.SqlClient;
using RestaurantDAL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantDAL
{
    /// <summary>
    /// Manage Report DAL
    /// </summary>
    public class ManageReportDAL : IManageReportDAL
    {
        private static string ConnectionString = Common.GetConnectionString();

        /// <summary>
        /// Search report
        /// </summary>
        /// <param name="searchReport">searchReport</param>
        /// <returns></returns>
        public ReportResponse SearchReport(SearchReport searchReport)
        {
            var response = new ReportResponse { IsSuccessFull = false };
            try
            {

                SqlParameter[] parameters2 = new SqlParameter[12];
                parameters2[0] = new SqlParameter("@CustomerName", searchReport.CustomerName);
                parameters2[1] = new SqlParameter("@OrderID", searchReport.OrderID);
                parameters2[2] = new SqlParameter("@RestaurantName", searchReport.RestaurantName);

                parameters2[3] = new SqlParameter("@Location", searchReport.Location);
                parameters2[4] = new SqlParameter("@ItemQuantity1", searchReport.ItemQuantity1 != 0 ? searchReport.ItemQuantity1 : null);
                parameters2[5] = new SqlParameter("@ItemQuantity2", searchReport.ItemQuantity2 != 0 ? searchReport.ItemQuantity2 : null);

                parameters2[6] = new SqlParameter("@OrderAmount1", searchReport.OrderAmount1 != 0 ? searchReport.OrderAmount1 : null);
                parameters2[7] = new SqlParameter("@OrderAmount2", searchReport.OrderAmount2 != 0 ? searchReport.OrderAmount2 : null);

                if (searchReport.OrderDateFrom != null)
                {
                    string x = searchReport.OrderDateFrom.ToString();
                    x = (Convert.ToDateTime(x)).ToString("yyyy-MM-dd").ToString();
                    parameters2[8] = new SqlParameter("@OrderDateFrom", x);
                }
                else
                {
                    parameters2[8] = new SqlParameter("@OrderDateFrom", searchReport.OrderDateFrom);
                }
                if (searchReport.OrderDateTo != null)
                {
                    string x = searchReport.OrderDateTo.ToString();
                    x = (Convert.ToDateTime(x)).ToString("yyyy-MM-dd").ToString();
                    parameters2[9] = new SqlParameter("@OrderDateTo", x);
                }
                else
                {
                    parameters2[9] = new SqlParameter("@OrderDateTo", searchReport.OrderDateTo);
                }

                if (string.IsNullOrEmpty(searchReport.SortBy))
                {
                    searchReport.SortBy = "CustomerName";
                }

                parameters2[10] = new SqlParameter("@SortBy", searchReport.SortBy);

                if (string.IsNullOrEmpty(searchReport.SortDirection))
                {
                    searchReport.SortDirection = "ASC";
                }
               
                parameters2[11] = new SqlParameter("@SortDirection", searchReport.SortDirection);

                var ds2 = SqlHelper.ExecuteDataset(ConnectionString, "USP_GetCustomerDynamically", parameters2);
                List<CustomerReport> customers = new();
                if (ds2?.Tables != null && ds2.Tables.Count > 0)
                {
                    customers = DataAccessHelper.ConvertToList<CustomerReport>(ds2.Tables[0]);
                }
                response.IsSuccessFull = true;
                response.CustomerReports = customers;
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
