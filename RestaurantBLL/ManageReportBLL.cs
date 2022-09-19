using RestaurantBLL.Interface;
using RestaurantDAL.Interface;
using RestaurantDTO.Response;

namespace RestaurantBLL
{
    public class ManageReportBLL : IManageReportBLL
    {
        private readonly IManageReportDAL _manageReportDAL;

        public ManageReportBLL(IManageReportDAL manageReportDAL)
        {
            _manageReportDAL = manageReportDAL;
        }

        public ReportResponse GetReport(int? CustomerID)
        {
            return _manageReportDAL.GetReport(CustomerID);
        }

      
    }
}
