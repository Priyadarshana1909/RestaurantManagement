using Microsoft.AspNetCore.Mvc;
using RestaurantDTO.Request;
using RestaurantDTO.Response;
using RestaurantWebApplication.Services.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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

        [HttpPost]
        public async Task<PartialViewResult> SearchCustomer(SearchReport searchReport)
        {

            // setup customize formatter for date time converter
            JsonSerializerSettings settings = new JsonSerializerSettings();
            IsoDateTimeConverter dateConverter = new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff"
            };
            settings.Converters.Add(dateConverter);

            var json = JsonConvert.SerializeObject(searchReport, settings);

            var response = await _apiService.ExecuteRequest<ReportResponse>("Report/SearchReport", HttpMethod.Post, json);

            if (response != null && response.IsSuccessFull)
            {
                return PartialView("_SearchCustomer", response.CustomerReports);
            }
            return PartialView("_SearchCustomer", new List<CustomerReport>());
        }

        #endregion

      

        #endregion
    }
}