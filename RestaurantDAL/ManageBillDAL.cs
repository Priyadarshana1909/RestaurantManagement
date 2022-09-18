using Microsoft.Data.SqlClient;
using RestaurantDAL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;
using System.Data;

namespace RestaurantDAL
{
    public class ManageBillDAL : IManageBillDAL
    {
        private static string ConnectionString = Common.GetConnectionString();

        public BillResponse GetBill(int? BillId)
        {
            var response = new BillResponse { IsSuccessFull = false };
            try
            {

                SqlParameter[] parameters2 = new SqlParameter[1];

                parameters2[0] = new SqlParameter("@BillID", BillId);

                var ds2 = SqlHelper.ExecuteDataset(ConnectionString, "USP_GetBill", parameters2);

                List<Bill> Bills = new();
                if (ds2 != null && ds2.Tables.Count > 0)
                    Bills = DataAccessHelper.ConvertToList<Bill>(ds2.Tables[0]);

                response.IsSuccessFull = true;
                response.Bills = Bills;
            }
            catch (Exception ex)
            {
                response.IsSuccessFull = false;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }


        public BaseResponse AddUpdateBill(AddUpdateBill AddUpdateBill)
        {
            var response = new BaseResponse();
            try
            {

                SqlParameter[] parameters2 = new SqlParameter[7];
                var rowCount = 0;
                parameters2[0] = new SqlParameter("@BillsID", AddUpdateBill.BillsID);
                parameters2[1] = new SqlParameter("@OrderID", AddUpdateBill.OrderID);
                parameters2[2] = new SqlParameter("@RestaurantID", AddUpdateBill.RestaurantID);
                parameters2[3] = new SqlParameter("@BillAmount", AddUpdateBill.BillAmount);
                parameters2[4] = new SqlParameter("@CustomerID", AddUpdateBill.CustomerID);               
                parameters2[5] = new SqlParameter("@IsDelete", AddUpdateBill.IsDelete);               
                parameters2[6] = new SqlParameter("@OutputID", rowCount)
                {
                    Direction = ParameterDirection.Output

                };

                SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "USP_Billis", parameters2);

                if (Convert.ToInt32(parameters2[6].Value) > 0)
                    response.IsSuccessFull = true;
            }
            catch (Exception ex)
            {
                response.IsSuccessFull = false;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }

        //private readonly IUnitOfWork<LocationMaster> _locationRepository;

        //public LocationMasterData(IUnitOfWork<LocationMaster> locationRepository)
        //{
        //    _locationRepository = locationRepository;
        //}

        //public BaseResponse AddUpdateCustomer(AddUpdateCustomer AddUpdateCustomer)
        //{
        //    var response = new BaseResponse();
        //    try
        //    {
        //        var context = _locationRepository.DbRepository().Context as UbiDbContext;
        //        SqlParameter[] parameters2 = new SqlParameter[10];

        //        parameters2[0] = new SqlParameter("@PageNumber", getLocationsSQLParams.PageNumber);
        //        parameters2[1] = new SqlParameter("@PageSize", getLocationsSQLParams.PageSize);
        //        parameters2[2] = new SqlParameter("@LocationName", !string.IsNullOrEmpty(getLocationsSQLParams.LocationName)
        //            ? getLocationsSQLParams.LocationName : null);
        //        parameters2[3] = new SqlParameter("@RegionName", !string.IsNullOrEmpty(getLocationsSQLParams.RegionName)
        //            ? getLocationsSQLParams.RegionName : null);
        //        parameters2[4] = new SqlParameter("@IsActive", getLocationsSQLParams.IsActive);
        //        parameters2[5] = new SqlParameter("@SortColumnName", getLocationsSQLParams.SortColumnName);
        //        parameters2[6] = new SqlParameter("@OrderByDirection", getLocationsSQLParams.OrderByDirection);
        //        parameters2[7] = new SqlParameter("@MainRootCompanyGuid", getLocationsSQLParams.MainRootCompanyGuid);

        //        parameters2[8] = new SqlParameter("@SearchId", !string.IsNullOrEmpty(getLocationsSQLParams.SearchId)
        //         ? getLocationsSQLParams.SearchId : null);

        //        parameters2[9] = new SqlParameter("@SearchLevel", getLocationsSQLParams.SearchLevel);

        //        var ds2 = SqlHelper.ExecuteDataset(connectionString, "SearchLocation", parameters2);

        //        if (ds2 != null && ds2.Tables.Count > 0)
        //            return DataAccessHelper.ConvertToList<GetLocationResponse>(ds2.Tables[0]);
        //    }
        //    catch (Exception ex)
        //    {
        //        response.IsSuccessFull = false;
        //        response.ErrorMessage = ex.Message;
        //    }
        //    return response;
        //}
    }
}
