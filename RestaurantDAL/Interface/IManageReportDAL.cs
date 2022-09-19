using RestaurantDTO.Response;

namespace RestaurantDAL.Interface
{
    public interface IManageReportDAL
    {
        ReportResponse GetReport(int? CustomerID);
    }
}
