using Microsoft.Data.SqlClient;
using RestaurantDAL.Interface;
using RestaurantDTO.Response;

namespace RestaurantDAL
{
    public class ManageMenuItemDAL : IManageMenuItemDAL
    {
        private static string ConnectionString = Common.GetConnectionString();
        public MenuItemResponse GetMenuItemFromRestaurantId(int RestaurantId)
        {
            var response = new MenuItemResponse { IsSuccessFull = false };
            try
            {

                SqlParameter[] parameters2 = new SqlParameter[1];

                parameters2[0] = new SqlParameter("@RestaurantID", RestaurantId);

                var ds2 = SqlHelper.ExecuteDataset(ConnectionString, "USP_GetMenuItemFromRestaurantId", parameters2);

                List<MenuItem> MenuItems = new();
                if (ds2 != null && ds2.Tables.Count > 0)
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
    }
}

