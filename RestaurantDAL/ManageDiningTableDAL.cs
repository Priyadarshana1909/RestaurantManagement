using Microsoft.Data.SqlClient;
using RestaurantDAL.Interface;
using RestaurantDTO.Response;

namespace RestaurantDAL
{
    public class ManageDiningTableDAL : IManageDiningTableDAL
    {
        private static string ConnectionString = Common.GetConnectionString();

        public DiningTableResponse GetDiningTablesFromRestaurantId(int RestaurantId)
        {
            var response = new DiningTableResponse { IsSuccessFull = false };
            try
            {

                SqlParameter[] parameters2 = new SqlParameter[1];

                parameters2[0] = new SqlParameter("@RestaurantID", RestaurantId);

                var ds2 = SqlHelper.ExecuteDataset(ConnectionString, "USP_GetDiningTableFromRestaurantId", parameters2);

                List<DiningTable> DiningTables = new();
                if (ds2 != null && ds2.Tables.Count > 0)
                    DiningTables = DataAccessHelper.ConvertToList<DiningTable>(ds2.Tables[0]);

                response.IsSuccessFull = true;
                response.DiningTables = DiningTables;
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
