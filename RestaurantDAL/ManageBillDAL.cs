using Microsoft.Data.SqlClient;
using RestaurantDAL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;
using System.Data;

namespace RestaurantDAL
{
    /// <summary>
    /// Manage bill dal
    /// </summary>
    public class ManageBillDAL : IManageBillDAL
    {
        private static string ConnectionString = Common.GetConnectionString();

        /// <summary>
        /// Get bill
        /// </summary>
        /// <param name="BillId"></param>
        /// <returns></returns>
        public BillResponse GetBill(int? BillId)
        {
            var response = new BillResponse { IsSuccessFull = false };
            try
            {

                SqlParameter[] parameters2 = new SqlParameter[1];

                parameters2[0] = new SqlParameter("@BillID", BillId);

                var ds2 = SqlHelper.ExecuteDataset(ConnectionString, "USP_GetBill", parameters2);

                if (ds2?.Tables != null && ds2.Tables.Count > 0)
                {
                    var Bills = DataAccessHelper.ConvertToList<Bill>(ds2.Tables[0]);
                    response.Bills = Bills;
                    response.IsSuccessFull = true;
                }
                else
                {
                    response.ErrorMessage = "Invalid bill no";
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
        /// Add / update / delete bill
        /// </summary>
        /// <param name="AddUpdateBill">AddUpdateBill</param>
        /// <returns></returns>
        public BaseResponse AddUpdateBill(AddUpdateBill AddUpdateBill)
        {
            var response = new BaseResponse();
            try
            {

                SqlParameter[] parameters2 = new SqlParameter[7];
                var rowCount = 0;
                parameters2[0] = new SqlParameter("@BillsID", AddUpdateBill.BillsID);
                parameters2[1] = new SqlParameter("@OrderID", AddUpdateBill.OrderID);
                parameters2[2] = new SqlParameter("@RestaurantID", AddUpdateBill.RestaurantID);
                parameters2[3] = new SqlParameter("@BillAmount", AddUpdateBill.BillAmount);
                parameters2[4] = new SqlParameter("@CustomerID", AddUpdateBill.CustomerID);               
                parameters2[5] = new SqlParameter("@IsDelete", AddUpdateBill.IsDelete);               
                parameters2[6] = new SqlParameter("@OutputID", rowCount)
                {
                    Direction = ParameterDirection.Output

                };

                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "USP_Billis", parameters2);

                if (Convert.ToInt32(parameters2[6].Value) > 0)
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
