using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RestaurantDAL.Interface;
using RestaurantDTO.Response;

namespace RestaurantDAL
{
    public class ManageCuisineDAL : IManageCuisineDAL
    {
        private static string ConnectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json")
            .Build().GetConnectionString("DefaultConnection");
        public CuisineResponse GetCuisines(int? CuisineId)
        {
            var response = new CuisineResponse { IsSuccessFull = false };
            try
            {
               
                SqlParameter[] parameters2 = new SqlParameter[1];

                parameters2[0] = new SqlParameter("@CuisineID", CuisineId);
               
                var ds2 = SqlHelper.ExecuteDataset(ConnectionString, "USP_GetCuisine", parameters2);

                List<Cuisine> cuisines = new();
                if (ds2 != null && ds2.Tables.Count > 0)
                    cuisines = DataAccessHelper.ConvertToList<Cuisine>(ds2.Tables[0]);

                response.IsSuccessFull = true;
                response.Cuisines = cuisines;
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
