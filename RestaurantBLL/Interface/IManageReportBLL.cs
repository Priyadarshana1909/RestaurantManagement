using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantBLL.Interface
{
    public interface IManageReportBLL
    {
        ReportResponse SearchReport(SearchReport searchReport);
    }
}
