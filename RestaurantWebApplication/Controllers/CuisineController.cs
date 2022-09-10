using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RestaurantDTO.Request;
using RestaurantDTO.Response;
using RestaurantWebApplication.Models;
using RestaurantWebApplication.Services.Interface;
using System.Linq;

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
            var response = await _apiService.ExecuteRequest<CuisineResponse>("Cuisine/GetCuisine/0", HttpMethod.Get, null);

            if (response != null && response.IsSuccessFull)
            {
                return View(response.Cuisines);
            }
            return View(new List<Cuisine>());
        }

        public async Task<IActionResult> Create()
        {
            var addUpdateCuisine = new AddUpdateCuisine();

            var restaurants = await _apiService.ExecuteRequest<RestaurantResponse>("Restaurant/GetRestaurant/0", HttpMethod.Get, null);

            if (restaurants?.Restaurants != null)
            {
                addUpdateCuisine.Restaurants = restaurants.Restaurants;
            }
           
          
            return View(addUpdateCuisine);
        }


    }
}