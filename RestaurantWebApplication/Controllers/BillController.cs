using Microsoft.AspNetCore.Mvc;
using RestaurantDTO.Request;
using RestaurantDTO.Response;
using RestaurantWebApplication.Services.Interface;

namespace RestaurantWebApplication.Controllers
{
    public class BillController : Controller
    {
        private readonly ILogger<BillController> _logger;
        private readonly IAPIService _apiService;

        public BillController(ILogger<BillController> logger, IAPIService aPiService)
        {
            _logger = logger;
            _apiService = aPiService;
        }

        #region "Action Results"

        #region "Index"
        public async Task<IActionResult> Index()
        {
            var response = await _apiService.ExecuteRequest<BillResponse>("Bill/GetBill/0", HttpMethod.Get, null);

            if (response != null && response.IsSuccessFull)
            {
                return View(response.Bills);
            }
            return View(new List<Bill>());
        }
        #endregion

        #region "Create"
        public async Task<IActionResult> Create()
        {
            var addUpdateBill = new AddUpdateBill();

            var restaurantResponse = await _apiService.ExecuteRequest<RestaurantResponse>("Restaurant/GetRestaurant/0", HttpMethod.Get, null);

            if (restaurantResponse?.Restaurants != null)
            {
                addUpdateBill.Restaurants = restaurantResponse.Restaurants;
            }

            var customerResponse = await _apiService.ExecuteRequest<CustomerResponse>("Customer/GetCustomer/0", HttpMethod.Get, null);

            if (customerResponse?.Customers != null)
            {
                addUpdateBill.Customers = customerResponse.Customers;
            }

            return View(addUpdateBill);
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
            addUpdateCuisine.CuisineId = id;
            addUpdateCuisine.IsDelete = true;
            addUpdateCuisine.CuisineName = "CuisineName";

            var response = await _apiService.ExecuteRequest<RestaurantResponse>("Cuisine/AddUpdateCuisine", HttpMethod.Post, addUpdateCuisine);

            return null;
        }
        #endregion

        #endregion
    }
}