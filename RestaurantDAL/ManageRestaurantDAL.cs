using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RestaurantDAL.Interface;
using RestaurantDTO.Response;

namespace RestaurantDAL
{
    public class ManageRestaurantDAL : IManageRestaurantDAL
    {
        private static string ConnectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json")
            .Build().GetConnectionString("DefaultConnection");
        public RestaurantResponse GetRestaurants(int? RestaurantId)
        {
            var response = new RestaurantResponse { IsSuccessFull = false };
            try
            {
               
                SqlParameter[] parameters2 = new SqlParameter[1];

                parameters2[0] = new SqlParameter("@RestaurantID", RestaurantId);
               
                var ds2 = SqlHelper.ExecuteDataset(ConnectionString, "USP_GetRestaurant", parameters2);

                List<Restaurant> restaurants = new();
                if (ds2 != null && ds2.Tables.Count > 0)
                    restaurants = DataAccessHelper.ConvertToList<Restaurant>(ds2.Tables[0]);

                response.IsSuccessFull = true;
                response.Restaurants = restaurants;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }
            return response;
        }
    }
}
