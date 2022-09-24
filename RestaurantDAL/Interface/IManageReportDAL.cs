using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantDAL.Interface
{
    public interface IManageReportDAL
    {
        ReportResponse SearchReport(SearchReport searchReport);
    }
}
