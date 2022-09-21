using Microsoft.AspNetCore.Mvc;
using RestaurantBLL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportController : Controller
    {
        private readonly IManageReportBLL _manageReportBLL;

        public ReportController(IManageReportBLL manageReportBLL)
        {
            _manageReportBLL = manageReportBLL;
        }

        [HttpGet]
        [Route("GetReport")]
        public async Task<ActionResult<ReportResponse>> GetReport()
        {
            var response = _manageReportBLL.GetReport(3);
            return response;
        }

        [HttpPost]
        [Route("SearchReport")]
        public async Task<ActionResult<ReportResponse>> SearchReport(SearchReport searchReport)
        {
            var response = _manageReportBLL.SearchReport(searchReport);
            return response;
        }

    }
}
