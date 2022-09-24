using Microsoft.AspNetCore.Mvc;
using RestaurantBLL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantWebAPI.Controllers
{
    /// <summary>
    /// Report controller - to get order and add update order
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ReportController : Controller
    {
        private readonly IManageReportBLL _manageReportBLL;

        /// <summary>
        /// Constructor for report controller
        /// </summary>
        /// <param name="manageReportBLL"></param>
        public ReportController(IManageReportBLL manageReportBLL)
        {
            _manageReportBLL = manageReportBLL;
        }

        #region Get Report for customer
        /// <summary>
        /// Get customer details based on search criteria
        /// </summary>
        /// <param name="searchReport"></param>
        /// <returns>ReportResponse</returns>
        [HttpPost]
        [Route("SearchReport")]
        public async Task<ActionResult<ReportResponse>> SearchReport(SearchReport searchReport)
        {
            var response = _manageReportBLL.SearchReport(searchReport);
            return response;
        }
        #endregion

    }
}
