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
        private readonly IUnitOfWork<EntityFrameworkUtility.Restaurant> _restaurantRepository;
        private readonly IUnitOfWork<EntityFrameworkUtility.RestaurantMenuItem> _restaurantMenuItemRepository;
        private readonly IUnitOfWork<EntityFrameworkUtility.DiningTable> _diningTableRepository;
        private readonly IUnitOfWork<EntityFrameworkUtility.DiningTableTrack> _diningTableTrackRepository;

        public ManageOrderDAL(IUnitOfWork<EntityFrameworkUtility.Order> orderRepository,
            IUnitOfWork<EntityFrameworkUtility.Restaurant> restaurantRepository,
            IUnitOfWork<EntityFrameworkUtility.RestaurantMenuItem> restaurantMenuItemRepository,
            IUnitOfWork<EntityFrameworkUtility.DiningTable> diningTableRepository,
            IUnitOfWork<EntityFrameworkUtility.DiningTableTrack> diningTableTrackRepository
            )
        {
            _orderRepository = orderRepository;
            _restaurantRepository = restaurantRepository;
            _restaurantMenuItemRepository = restaurantMenuItemRepository;
            _diningTableRepository = diningTableRepository;
            _diningTableTrackRepository = diningTableTrackRepository;  

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

                var Orders = _orderRepository.DbRepository().GetQueryWithIncludes(OrderId == null ? null : OrderPredicate, null, new string[] { "Restaurant", "RestaurantMenuItem", "Restaurant.DiningTables" }).ToList();

                if (Orders != null && Orders.Any())
                {
                    foreach (var orderItem in Orders)
                    {
                        response.Orders.Add(new Order()
                        {
                            OrderID = orderItem.OrderID,
                            OrderDate = orderItem.OrderDate,
                            RestaurantID = orderItem.Restaurant?.RestaurantID ?? 0,
                            MenuItemID = orderItem.RestaurantMenuItem?.MenuItemID ?? 0,
                            ItemQuantity = orderItem.ItemQuantity,
                            ItemPrice = orderItem.RestaurantMenuItem?.ItemPrice ?? 0,
                            OrderAmount = orderItem.OrderAmount,
                            DiningTableID = orderItem.DiningTable?.DiningTableID ?? 0,
                            RestaurantName = orderItem.Restaurant?.RestaurantName, 
                            ItemName = orderItem.RestaurantMenuItem?.ItemName
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
                if (AddUpdateOrder.OrderID == 0)
                {
                    Expression<Func<EntityFrameworkUtility.Restaurant, bool>> RestaurantPredicate = PredicateBuilder.New<EntityFrameworkUtility.Restaurant>(true);

                    RestaurantPredicate = RestaurantPredicate.And(x => x.RestaurantID == AddUpdateOrder.RestaurantID);

                    var restaurants = _restaurantRepository.DbRepository().GetQueryWithIncludes(RestaurantPredicate, null, null).ToList();

                    if (restaurants == null || !restaurants.Any())
                    {
                        response.ErrorMessage = "Invalid restaurant id";
                        return response;
                    }

                    Expression<Func<EntityFrameworkUtility.RestaurantMenuItem, bool>> RestaurantMenuItemPredicate = PredicateBuilder.New<EntityFrameworkUtility.RestaurantMenuItem>(true);

                    RestaurantMenuItemPredicate = RestaurantMenuItemPredicate.And(x => x.MenuItemID == AddUpdateOrder.MenuItemID);

                    var restaurantMenuItems = _restaurantMenuItemRepository.DbRepository().GetQueryWithIncludes(RestaurantMenuItemPredicate, null, null).ToList();

                    if (restaurantMenuItems == null || !restaurantMenuItems.Any())
                    {
                        response.ErrorMessage = "Invalid restaurant menu item id";
                        return response;
                    }


                    Expression<Func<EntityFrameworkUtility.DiningTable, bool>> DiningTablePredicate = PredicateBuilder.New<EntityFrameworkUtility.DiningTable>(true);

                    DiningTablePredicate = DiningTablePredicate.And(x => x.DiningTableID == AddUpdateOrder.DiningTableID);

                    var diningTables = _diningTableRepository.DbRepository().GetQueryWithIncludes(DiningTablePredicate, null, new string[] { "DiningTableTrack" }).ToList();

                    if (diningTables == null || !diningTables.Any())
                    {
                        response.ErrorMessage = "Invalid dining table id";
                        return response;
                    }


                    var order = new EntityFrameworkUtility.Order();
                    order.ItemQuantity = AddUpdateOrder.ItemQuantity ?? 0;
                    order.Restaurant = restaurants.FirstOrDefault();
                    order.RestaurantMenuItem = restaurantMenuItems.FirstOrDefault();
                    order.OrderAmount = restaurantMenuItems.FirstOrDefault().ItemPrice * order.ItemQuantity;
                    order.OrderDate = DateTime.Now;
                    order.DiningTable = diningTables.FirstOrDefault();

                    _orderRepository.DbRepository().Insert(order);

                    _ = _orderRepository.SaveAsync().Result;

                    if (Convert.ToInt32(order.OrderID) > 0)
                    {
                        var diningTableTrack = diningTables.FirstOrDefault().DiningTableTrack;

                        if (diningTableTrack != null)
                        {
                            diningTableTrack.TableStatus = "Occupied";
                            _diningTableTrackRepository.DbRepository().Update(diningTableTrack);
                            _ = _diningTableTrackRepository.SaveAsync().Result;
                        }
                        response.IsSuccessFull = true;
                    }
                }
                else
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

                    if (Convert.ToInt32(parameters2[7].Value) > 0)
                        response.IsSuccessFull = true;
                }
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
