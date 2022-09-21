using Microsoft.Data.SqlClient;
using RestaurantDAL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;
using System.Data;

namespace RestaurantDAL
{
    public class ManageCuisineDAL : IManageCuisineDAL
    {
        private static string ConnectionString = Common.GetConnectionString();

        //public BaseResponse AddUpdateCuisine(AddUpdateCuisine addUpdateCuisine)
        //{
        //    throw new NotImplementedException();
        //}

        public BaseResponse AddUpdateCuisine(AddUpdateCuisine addUpdateCuisine)
        {
            var response = new BaseResponse();
            try
            {

                SqlParameter[] parameters2 = new SqlParameter[5];
                var newlyaddedValue = 0;
                parameters2[0] = new SqlParameter("@CuisineID", addUpdateCuisine.CuisineId);
                parameters2[1] = new SqlParameter("@RestaurantID", addUpdateCuisine.RestaurantId);
                parameters2[2] = new SqlParameter("@CuisineName", addUpdateCuisine.CuisineName);
                parameters2[3] = new SqlParameter("@IsDelete", addUpdateCuisine.IsDelete);
                parameters2[4] = new SqlParameter("@OutputCuisineId", newlyaddedValue)
                {
                    Direction = ParameterDirection.Output

                };

                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "USP_Cuisine", parameters2);
                response.IsSuccessFull = true;
            }
            catch (Exception ex)
            {
                response.IsSuccessFull = false;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }

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
