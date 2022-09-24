using RestaurantBLL.Interface;
using RestaurantDAL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantBLL
{
    /// <summary>
    /// Manage report bll
    /// </summary>
    public class ManageReportBLL : IManageReportBLL
    {
        private readonly IManageReportDAL _manageReportDAL;

        public ManageReportBLL(IManageReportDAL manageReportDAL)
        {
            _manageReportDAL = manageReportDAL;
        }

        /// <summary>
        /// Search customer report
        /// </summary>
        /// <param name="searchReport"></param>
        /// <returns></returns>
        public ReportResponse SearchReport(SearchReport searchReport)
        {
            return _manageReportDAL.SearchReport(searchReport);
        }
        
    }
}
