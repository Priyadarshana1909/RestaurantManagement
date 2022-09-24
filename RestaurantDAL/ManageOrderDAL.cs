using Microsoft.Data.SqlClient;
using RestaurantDAL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;
using System.Data;

namespace RestaurantDAL
{
    /// <summary>
    /// Manage order dal
    /// </summary>
    public class ManageOrderDAL : IManageOrderDAL
    {
        private static string ConnectionString = Common.GetConnectionString();

        /// <summary>
        /// Get order
        /// </summary>
        /// <param name="OrderId">OrderId</param>
        /// <returns></returns>
        public OrderResponse GetOrder(int? OrderId)
        {
            var response = new OrderResponse { IsSuccessFull = false };
            try
            {

                SqlParameter[] parameters2 = new SqlParameter[1];

                parameters2[0] = new SqlParameter("@OrderID", OrderId);

                var ds2 = SqlHelper.ExecuteDataset(ConnectionString, "USP_GetOrder", parameters2);

                List<Order> Orders = new();
                if (ds2 != null && ds2.Tables.Count > 0)
                    Orders = DataAccessHelper.ConvertToList<Order>(ds2.Tables[0]);

                response.IsSuccessFull = true;
                response.Orders = Orders;
            }
            catch (Exception ex)
            {
                response.IsSuccessFull = false;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// Add / update / delete order
        /// </summary>
        /// <param name="AddUpdateOrder">AddUpdateOrder</param>
        /// <returns></returns>
        public BaseResponse AddUpdateOrder(AddUpdateOrder AddUpdateOrder)
        {
            var response = new BaseResponse();
            try
            {

                SqlParameter[] parameters2 = new SqlParameter[8];
                var rowCount = 0;
                parameters2[0] = new SqlParameter("@OrderID", AddUpdateOrder.OrderID);
                parameters2[1] = new SqlParameter("@RestaurantID", AddUpdateOrder.RestaurantID);
                parameters2[2] = new SqlParameter("@MenuItemID", AddUpdateOrder.MenuItemID);
                parameters2[3] = new SqlParameter("@ItemQuantity", AddUpdateOrder.ItemQuantity);
                //parameters2[4] = new SqlParameter("@OrderAmount", AddUpdateOrder.OrderAmount);
                parameters2[4] = new SqlParameter("@DiningTableID", AddUpdateOrder.DiningTableID);
                parameters2[5] = new SqlParameter("@IsDelete", AddUpdateOrder.IsDelete);
                parameters2[6] = new SqlParameter("@OrderDate", AddUpdateOrder.OrderDate);
                parameters2[7] = new SqlParameter("@OutputId", rowCount)
                {
                    Direction = ParameterDirection.Output

                };

                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "USP_Order", parameters2);

                if(Convert.ToInt32(parameters2[7].Value) > 0)
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
