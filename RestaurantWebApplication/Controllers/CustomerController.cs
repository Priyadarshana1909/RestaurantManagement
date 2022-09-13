using Microsoft.AspNetCore.Mvc;
using RestaurantDTO.Request;
using RestaurantDTO.Response;
using RestaurantWebApplication.Services.Interface;

namespace RestaurantWebApplication.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly IAPIService _apiService;

        public CustomerController(ILogger<CustomerController> logger, IAPIService aPiService)
        {
            _logger = logger;
            _apiService = aPiService;
        }

        #region "Action Results"

        #region "Index"
        public async Task<IActionResult> Index()
        {
            var response = await _apiService.ExecuteRequest<CustomerResponse>("Customer/GetCustomer/0", HttpMethod.Get, null);

            if (response != null && response.IsSuccessFull)
            {
                return View(response.Customers);
            }
            return View(new List<Customer>());
        }
        #endregion

        #region "Create"
        public async Task<IActionResult> Create()
        {
            var addUpdateCuisine = new AddUpdateCustomer();

            var restaurants = await _apiService.ExecuteRequest<RestaurantResponse>("Restaurant/GetRestaurant/0", HttpMethod.Get, null);

            if (restaurants?.Restaurants != null)
            {
                addUpdateCuisine.Restaurants = restaurants.Restaurants;
            }
            
            return View(addUpdateCuisine);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddUpdateCustomer AddUpdateCustomer)
        {
            if (ModelState.IsValid)
            {
                var response = await _apiService.ExecuteRequest<CustomerResponse>("Customer/AddUpdateCustomer", HttpMethod.Post, AddUpdateCustomer);

                if (response != null && response.IsSuccessFull)
                {
                    TempData["Message"] = "Customer saved successfully";
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
                    AddUpdateCustomer.Restaurants = restaurants.Restaurants;
                }
            }

            return View(AddUpdateCustomer);
        }
        #endregion

        #region "Edit"
        public async Task<IActionResult> Edit(int id)
        {
            var addUpdateCustomer = new AddUpdateCustomer();

            var restaurants = await _apiService.ExecuteRequest<RestaurantResponse>("Restaurant/GetRestaurant/0", HttpMethod.Get, null);

            if (restaurants?.Restaurants != null)
            {
                addUpdateCustomer.Restaurants = restaurants.Restaurants;
            }

            var response = await _apiService.ExecuteRequest<CustomerResponse>("Customer/GetCustomer/" + id, HttpMethod.Get, null);

            if (response != null)
            {
                addUpdateCustomer.RestaurantId = response.Customers[0].RestaurantID;
                addUpdateCustomer.CustomerId = id;
                addUpdateCustomer.CustomerName = response.Customers[0].CustomerName;
                addUpdateCustomer.MobileNo = response.Customers[0].MobileNo;
            }
            return View(addUpdateCustomer);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AddUpdateCustomer AddUpdateCustomer)
        {
            if (ModelState.IsValid)
            {
                var response = await _apiService.ExecuteRequest<RestaurantResponse>("Customer/AddUpdateCustomer", HttpMethod.Post, AddUpdateCustomer);

                if (response != null && response.IsSuccessFull)
                {
                    TempData["Message"] = "Customer updated successfully";
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
                    AddUpdateCustomer.Restaurants = restaurants.Restaurants;
                }
            }

            return View(AddUpdateCustomer);
        }
        #endregion

        #region "Delete"
   

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            AddUpdateCustomer addUpdateCuisine = new();
            addUpdateCuisine.CustomerId = id;
            addUpdateCuisine.IsDelete = true;
            addUpdateCuisine.CustomerName = "CustomerName";
            addUpdateCuisine.MobileNo = "1234567890";

            var response = await _apiService.ExecuteRequest<CustomerResponse>("Customer/AddUpdateCustomer", HttpMethod.Post, addUpdateCuisine);
            return null;
            
        }
        #endregion

        #endregion
    }
}