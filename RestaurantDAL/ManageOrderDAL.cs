using Microsoft.Extensions.Configuration;
using RestaurantDAL.EntityFrameworkUtility;
using RestaurantDAL.Interface;
using RestaurantDTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantDAL
{
    public class ManageOrderDAL
    {
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
