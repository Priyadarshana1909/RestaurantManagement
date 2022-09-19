using RestaurantDTO.Response;

namespace RestaurantBLL.Interface
{
    public interface IManageReportBLL
    {
        ReportResponse GetReport(int? CustomerID);

      
    }
}
