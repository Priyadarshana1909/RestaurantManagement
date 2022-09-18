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

            var orderResponse = await _apiService.ExecuteRequest<OrderResponse>("Order/GetOrder/0", HttpMethod.Get, null);

            if (orderResponse?.Orders != null)
            {
                addUpdateBill.Orders = orderResponse.Orders;
            }

            return View(addUpdateBill);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddUpdateBill addUpdateBill)
        {
            if (ModelState.IsValid)
            {
                var response = await _apiService.ExecuteRequest<RestaurantResponse>("Bill/AddUpdateBill", HttpMethod.Post, addUpdateBill);

                if (response != null && response.IsSuccessFull)
                {
                    TempData["Message"] = "Bill saved successfully";
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
                    addUpdateBill.Restaurants = restaurants.Restaurants;
                }
            }

            return View(addUpdateBill);
        }
        #endregion

        #region "Edit"


        public async Task<IActionResult> Edit(int id)
        {
            var addUpdateBill = new AddUpdateBill();

            var restaurants = await _apiService.ExecuteRequest<RestaurantResponse>("Restaurant/GetRestaurant/0", HttpMethod.Get, null);

            if (restaurants?.Restaurants != null)
            {
                addUpdateBill.Restaurants = restaurants.Restaurants;
            }

            var orders = await _apiService.ExecuteRequest<OrderResponse>("Order/GetOrder/0", HttpMethod.Get, null);

            if (orders?.Orders != null)
            {
                addUpdateBill.Orders = orders.Orders;
            }

            var customers = await _apiService.ExecuteRequest<CustomerResponse>("Customer/GetCustomer/0", HttpMethod.Get, null);

            if (customers?.Customers != null)
            {
                addUpdateBill.Customers = customers.Customers;
            }

            var response = await _apiService.ExecuteRequest<BillResponse>("Bill/GetBill/" + id, HttpMethod.Get, null);

            if (response != null)
            {
                addUpdateBill.OrderID = response.Bills[0].OrderID;
                addUpdateBill.BillsID = id;
                addUpdateBill.RestaurantID = response.Bills[0].RestaurantID;
                addUpdateBill.BillAmount = response.Bills[0].BillAmount;
                addUpdateBill.CustomerID = response.Bills[0].CustomerID;
            }
            return View(addUpdateBill);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AddUpdateBill addUpdateBill)
        {
            if (ModelState.IsValid)
            {
                var response = await _apiService.ExecuteRequest<RestaurantResponse>("Bill/AddUpdateBill", HttpMethod.Post, addUpdateBill);

                if (response != null && response.IsSuccessFull)
                {
                    TempData["Message"] = "Bill updated successfully";
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
                    addUpdateBill.Restaurants = restaurants.Restaurants;
                }

                var orders = await _apiService.ExecuteRequest<OrderResponse>("Order/GetOrder/0", HttpMethod.Get, null);

                if (orders?.Orders != null)
                {
                    addUpdateBill.Orders = orders.Orders;
                }

                var customers = await _apiService.ExecuteRequest<CustomerResponse>("Customer/GetCustomer/0", HttpMethod.Get, null);

                if (customers?.Customers != null)
                {
                    addUpdateBill.Customers = customers.Customers;
                }
            }

            return View(addUpdateBill);
        }
        #endregion

        #region "Delete"
   

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _apiService.ExecuteRequest<BillResponse>("Bill/GetBill/" + id, HttpMethod.Get, null);
            AddUpdateBill addUpdateBill = new();

            if (response != null)
            {
                addUpdateBill.OrderID = response.Bills[0].OrderID;
                addUpdateBill.BillsID = id;
                addUpdateBill.RestaurantID = response.Bills[0].RestaurantID;
                addUpdateBill.BillAmount = response.Bills[0].BillAmount;
                addUpdateBill.CustomerID = response.Bills[0].CustomerID;    
            }

            addUpdateBill.IsDelete = true;

            var deleteBillResponse = await _apiService.ExecuteRequest<BillResponse>("Bill/AddUpdateBill", HttpMethod.Post, addUpdateBill);
            return Json(deleteBillResponse);
        }
        #endregion

        #endregion
    }
}