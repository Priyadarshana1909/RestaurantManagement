using LinqKit;
using Microsoft.Data.SqlClient;
using RestaurantDAL.Interface;
using RestaurantDTO.Response;
using System.Data;
using System.Linq.Expressions;

namespace RestaurantDAL
{
    /// <summary>
    /// Get menu item DAL
    /// </summary>
    public class ManageMenuItemDAL : IManageMenuItemDAL
    {
        private static string ConnectionString = Common.GetConnectionString();

        private readonly IUnitOfWork<EntityFrameworkUtility.RestaurantMenuItem> _menuItemRepository;

        public ManageMenuItemDAL(IUnitOfWork<EntityFrameworkUtility.RestaurantMenuItem> menuItemRepository)
        {
            _menuItemRepository = menuItemRepository;
        }

        /// <summary>
        /// Get menu item from restautant id
        /// </summary>
        /// <param name="RestaurantId"></param>
        /// <returns></returns>
        public MenuItemResponse GetMenuItemFromRestaurantId(int RestaurantId)
        {
            var response = new MenuItemResponse { IsSuccessFull = false };
            try
            {

                SqlParameter[] parameters2 = new SqlParameter[1];

                parameters2[0] = new SqlParameter("@RestaurantID", RestaurantId);

                var ds2 = SqlHelper.ExecuteDataset(ConnectionString, "USP_GetMenuItemFromRestaurantId", parameters2);

                List<MenuItem> MenuItems = new();
                if (ds2?.Tables != null && ds2.Tables.Count > 0)
                    MenuItems = DataAccessHelper.ConvertToList<MenuItem>(ds2.Tables[0]);

                response.IsSuccessFull = true;
                response.MenuItems = MenuItems;
            }
            catch (Exception ex)
            {
                response.IsSuccessFull = false;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// Get order
        /// </summary>
        /// <param name="OrderId">OrderId</param>
        /// <returns></returns>
        public MenuItemResponse GetMenuItemPrice(int? MenuItemId)
        {
            var response = new MenuItemResponse { IsSuccessFull = false };

            try
            {

                Expression<Func<EntityFrameworkUtility.RestaurantMenuItem, bool>> MenuItemPredicate = PredicateBuilder.New<EntityFrameworkUtility.RestaurantMenuItem>(true);

                if (MenuItemId != null)
                    MenuItemPredicate = MenuItemPredicate.And(x => x.MenuItemID == MenuItemId.Value);

                var MenuItems = _menuItemRepository.DbRepository().GetQueryWithIncludes(MenuItemId == null ? null : MenuItemPredicate, null, new string[] {}).ToList();

                if (MenuItems != null && MenuItems.Any())
                {
                    foreach (var orderItem in MenuItems)
                    {
                        response.MenuItems.Add(new MenuItem()
                        {
                            MenuItemID = orderItem.MenuItemID,                            
                            ItemPrice = orderItem.ItemPrice,
                            ItemName = orderItem.ItemName
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

        
    }
}

