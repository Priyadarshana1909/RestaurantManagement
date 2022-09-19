using Microsoft.AspNetCore.Mvc;
using RestaurantDTO.Response;
using RestaurantWebApplication.Services.Interface;

namespace RestaurantWebApplication.Controllers
{
    public class ReportController : Controller
    {
        private readonly ILogger<ReportController> _logger;
        private readonly IAPIService _apiService;

        public ReportController(ILogger<ReportController> logger, IAPIService aPiService)
        {
            _logger = logger;
            _apiService = aPiService;
        }

        #region "Action Results"

        #region "Index"
        public async Task<IActionResult> Index()
        {
            var response = await _apiService.ExecuteRequest<ReportResponse>("Report/GetReport", HttpMethod.Get, null);

            if (response != null && response.IsSuccessFull)
            {
                return View(response.CustomerReports);
            }
            return View(new List<CustomerReport>());
        }
        #endregion    
        
        #endregion
    }
}