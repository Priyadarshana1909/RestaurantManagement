using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantBLL.Interface
{
    public interface IManageReportBLL
    {
        ReportResponse GetReport(int? CustomerID);

        ReportResponse SearchReport(SearchReport searchReport);
    }
}
