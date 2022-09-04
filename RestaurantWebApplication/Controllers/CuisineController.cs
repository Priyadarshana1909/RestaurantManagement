using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RestaurantDTO.Response;
using RestaurantWebApplication.Models;
using RestaurantWebApplication.Services.Interface;

namespace RestaurantWebApplication.Controllers
{
    public class CuisineController : Controller
    {
        private readonly ILogger<CuisineController> _logger;
        private readonly IAPIService _apiService;

        public CuisineController(ILogger<CuisineController> logger, IAPIService aPiService)
        {
            _logger = logger;
            _apiService = aPiService;
        }
        
        public async Task<IActionResult> Index()
        {
            var response = await _apiService.ExecuteRequest<CuisineResponse>("Cuisine", HttpMethod.Get, null);
            return View(response.Cuisines);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}