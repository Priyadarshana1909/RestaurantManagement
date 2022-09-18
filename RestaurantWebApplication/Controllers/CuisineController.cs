using Microsoft.AspNetCore.Mvc;
using RestaurantDTO.Request;
using RestaurantDTO.Response;
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

        #region "Action Results"

        #region "Index"
        public async Task<IActionResult> Index()
        {
            var response = await _apiService.ExecuteRequest<CuisineResponse>("Cuisine/GetCuisine/0", HttpMethod.Get, null);

            if (response != null && response.IsSuccessFull)
            {
                return View(response.Cuisines);
            }
            return View(new List<Cuisine>());
        }
        #endregion

        #region "Create"
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

        [HttpPost]
        public async Task<IActionResult> Create(AddUpdateCuisine addUpdateCuisine)
        {
            if (ModelState.IsValid)
            {
                var response = await _apiService.ExecuteRequest<RestaurantResponse>("Cuisine/AddUpdateCuisine", HttpMethod.Post, addUpdateCuisine);

                if (response != null && response.IsSuccessFull)
                {
                    TempData["Message"] = "Cuisine saved successfully";
                }
                else
                {
                    TempData["Message"] = response?.ErrorMessage;
                }

                return RedirectToAction("Index");
            }
            else
            {
                var restaurants = await _apiService.ExecuteRequest<RestaurantResponse>("Restaurant/GetRestaurant/0", HttpMethod.Get, null);

                if (restaurants?.Restaurants != null)
                {
                    addUpdateCuisine.Restaurants = restaurants.Restaurants;
                }
            }

            return View(addUpdateCuisine);
        }
        #endregion

        #region "Edit"
        public async Task<IActionResult> Edit(int id)
        {
            var addUpdateCuisine = new AddUpdateCuisine();

            var restaurants = await _apiService.ExecuteRequest<RestaurantResponse>("Restaurant/GetRestaurant/0", HttpMethod.Get, null);

            if (restaurants?.Restaurants != null)
            {
                addUpdateCuisine.Restaurants = restaurants.Restaurants;
            }

            var response = await _apiService.ExecuteRequest<CuisineResponse>("Cuisine/GetCuisine/" + id, HttpMethod.Get, null);

            if (response != null)
            {
                addUpdateCuisine.RestaurantId = response.Cuisines[0].RestaurantID;
                addUpdateCuisine.CuisineId = id;
                addUpdateCuisine.CuisineName = response.Cuisines[0].CuisineName;
            }
            return View(addUpdateCuisine);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AddUpdateCuisine addUpdateCuisine)
        {
            if (ModelState.IsValid)
            {
                var response = await _apiService.ExecuteRequest<RestaurantResponse>("Cuisine/AddUpdateCuisine", HttpMethod.Post, addUpdateCuisine);

                if (response != null && response.IsSuccessFull)
                {
                    TempData["Message"] = "Cuisine updated successfully";
                }
                else
                {
                    TempData["Message"] = response?.ErrorMessage;
                }

                return RedirectToAction("Index");
            }
            else
            {
                var restaurants = await _apiService.ExecuteRequest<RestaurantResponse>("Restaurant/GetRestaurant/0", HttpMethod.Get, null);

                if (restaurants?.Restaurants != null)
                {
                    addUpdateCuisine.Restaurants = restaurants.Restaurants;
                }
            }

            return View(addUpdateCuisine);
        }
        #endregion

        #region "Delete"
   

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            AddUpdateCuisine addUpdateCuisine = new();
            var response = await _apiService.ExecuteRequest<CuisineResponse>("Cuisine/GetCuisine/" + id, HttpMethod.Get, null);

            if (response != null)
            {
                addUpdateCuisine.RestaurantId = response.Cuisines[0].RestaurantID;
                addUpdateCuisine.CuisineId = id;
                addUpdateCuisine.CuisineName = response.Cuisines[0].CuisineName;
            }

            addUpdateCuisine.IsDelete = true;
            var CuisineDeleteResponse = await _apiService.ExecuteRequest<RestaurantResponse>("Cuisine/AddUpdateCuisine", HttpMethod.Post, addUpdateCuisine);

            return Json(CuisineDeleteResponse);
        }
        #endregion

        #endregion
    }
}