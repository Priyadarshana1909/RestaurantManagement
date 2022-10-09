using LinqKit;
using Microsoft.Data.SqlClient;
using RestaurantDAL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;
using System.Data;
using System.Linq.Expressions;

namespace RestaurantDAL
{
    /// <summary>
    /// Manage order dal
    /// </summary>
    public class ManageOrderDAL : IManageOrderDAL
    {
        private static string ConnectionString = Common.GetConnectionString();


        private readonly IUnitOfWork<EntityFrameworkUtility.Order> _orderRepository;

        public ManageOrderDAL(IUnitOfWork<EntityFrameworkUtility.Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }


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

                Expression<Func<EntityFrameworkUtility.Order, bool>> OrderPredicate = PredicateBuilder.New<EntityFrameworkUtility.Order>(true);

                if (OrderId != null)
                    OrderPredicate = OrderPredicate.And(x => x.OrderID == OrderId.Value);

                var Orders = _orderRepository.DbRepository().GetQueryWithIncludes(OrderId == null ? null : OrderPredicate, null, new string[] { "Restaurant", "RestaurantMenuItem" }).ToList();

                if (Orders != null && Orders.Any())
                {
                    foreach (var orderItem in Orders)
                    {
                        response.Orders.Add(new Order()
                        {
                            OrderID = orderItem.OrderID,
                            OrderDate = orderItem.OrderDate,
                            RestaurantID = orderItem.Restaurant.RestaurantID,
                            MenuItemID = orderItem.RestaurantMenuItem.MenuItemID,
                            ItemQuantity = orderItem.ItemQuantity,
                            ItemPrice = orderItem.RestaurantMenuItem.ItemPrice,
                            OrderAmount = orderItem.OrderAmount,
                            DiningTableID = orderItem.DiningTableID,
                            RestaurantName = orderItem.Restaurant.RestaurantName,
                            ItemName = orderItem.RestaurantMenuItem.ItemName
                        });
                    }
                }
                response.IsSuccessFull = true;
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
