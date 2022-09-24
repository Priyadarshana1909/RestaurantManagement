using Microsoft.Data.SqlClient;
using RestaurantDAL.Interface;
using RestaurantDTO.Response;

namespace RestaurantDAL
{
    /// <summary>
    /// Manage restaurant DAL
    /// </summary>
    public class ManageRestaurantDAL : IManageRestaurantDAL
    {
        private static string ConnectionString = Common.GetConnectionString();
        
        /// <summary>
        /// Get restaurants details based on restaurant id
        /// </summary>
        /// <param name="RestaurantId"></param>
        /// <returns></returns>
        public RestaurantResponse GetRestaurants(int? RestaurantId)
        {
            var response = new RestaurantResponse { IsSuccessFull = false };
            try
            {
               
                SqlParameter[] parameters2 = new SqlParameter[1];

                parameters2[0] = new SqlParameter("@RestaurantID", RestaurantId);
               
                var ds2 = SqlHelper.ExecuteDataset(ConnectionString, "USP_GetRestaurant", parameters2);

                if (ds2?.Tables != null && ds2.Tables.Count > 0)
                {
                    var restaurants = DataAccessHelper.ConvertToList<Restaurant>(ds2.Tables[0]);
                    response.IsSuccessFull = true;
                    response.Restaurants = restaurants;
                }
                else
                {
                    response.ErrorMessage = "Invalid restaurant id";
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
