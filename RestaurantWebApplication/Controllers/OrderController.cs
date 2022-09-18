using Microsoft.AspNetCore.Mvc;
using RestaurantDTO.Request;
using RestaurantDTO.Response;
using RestaurantWebApplication.Services.Interface;

namespace RestaurantWebApplication.Controllers
{
    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IAPIService _apiService;

        public OrderController(ILogger<OrderController> logger, IAPIService aPiService)
        {
            _logger = logger;
            _apiService = aPiService;
        }

        #region "Index"
        public async Task<IActionResult> Index()
        {
            var response = await _apiService.ExecuteRequest<OrderResponse>("Order/GetOrder/0", HttpMethod.Get, null);

            if (response != null && response.IsSuccessFull)
            {
                return View(response.Orders);
            }
            return View(new List<Order>());
        }
        #endregion

        #region "Create"
        public async Task<IActionResult> Create()
        {
            var addUpdateOrder = new AddUpdateOrder();

            var restaurants = await _apiService.ExecuteRequest<RestaurantResponse>("Restaurant/GetRestaurant/0", HttpMethod.Get, null);

            if (restaurants?.Restaurants != null && restaurants.Restaurants.Any())
            {
                addUpdateOrder.Restaurants = restaurants.Restaurants;

                var GetMenuItemResponse = _getMenuItems(restaurants.Restaurants.First().RestaurantID).Result;

                if (GetMenuItemResponse?.MenuItems != null && GetMenuItemResponse.MenuItems.Any())
                    addUpdateOrder.MenuItems = GetMenuItemResponse.MenuItems;

                var GetDiningTableResponse = _getDiningTables(restaurants.Restaurants.First().RestaurantID).Result;

                if (GetDiningTableResponse?.DiningTables != null && GetDiningTableResponse.DiningTables.Any())
                    addUpdateOrder.DiningTables = GetDiningTableResponse.DiningTables;
            }

            return View(addUpdateOrder);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddUpdateOrder addUpdateOrder)
        {
            if (ModelState.IsValid)
            {
                var response = await _apiService.ExecuteRequest<RestaurantResponse>("Order/AddUpdateOrder", HttpMethod.Post, addUpdateOrder);

                if (response != null && response.IsSuccessFull)
                {
                    TempData["Message"] = "Order saved successfully";
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

                if (restaurants?.Restaurants != null && restaurants.Restaurants.Any())
                {
                    addUpdateOrder.Restaurants = restaurants.Restaurants;

                    var GetMenuItemResponse = _getMenuItems(addUpdateOrder.RestaurantID).Result;

                    if (GetMenuItemResponse?.MenuItems != null && GetMenuItemResponse.MenuItems.Any())
                        addUpdateOrder.MenuItems = GetMenuItemResponse.MenuItems;

                    var GetDiningTableResponse = _getDiningTables(addUpdateOrder.RestaurantID).Result;

                    if (GetDiningTableResponse?.DiningTables != null && GetDiningTableResponse.DiningTables.Any())
                        addUpdateOrder.DiningTables = GetDiningTableResponse.DiningTables;
                }
            }

            return View(addUpdateOrder);
        }
        #endregion

        #region "Edit"
        public async Task<IActionResult> Edit(int id)
        {
            var addUpdateOrder = await GetExisingOrder(id);
            return View(addUpdateOrder);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AddUpdateOrder addUpdateOrder)
        {
            if (ModelState.IsValid)
            {
                var response = await _apiService.ExecuteRequest<RestaurantResponse>("Order/AddUpdateOrder", HttpMethod.Post, addUpdateOrder);

                if (response != null && response.IsSuccessFull)
                {
                    TempData["Message"] = "Order saved successfully";
                }
                else
                {
                    TempData["Message"] = response?.ErrorMessage;
                }

                return RedirectToAction("Index");
            }
            else
            {
                var response = await _apiService.ExecuteRequest<OrderResponse>("Order/GetOrder/" + addUpdateOrder.OrderID, HttpMethod.Get, null);

                if (response?.Orders != null && response.Orders.Any())
                {
                    var restaurants = await _apiService.ExecuteRequest<RestaurantResponse>("Restaurant/GetRestaurant/0", HttpMethod.Get, null);

                    if (restaurants?.Restaurants != null && restaurants.Restaurants.Any())
                    {
                        addUpdateOrder.Restaurants = restaurants.Restaurants;

                        var GetMenuItemResponse = _getMenuItems(addUpdateOrder.RestaurantID).Result;

                        if (GetMenuItemResponse?.MenuItems != null && GetMenuItemResponse.MenuItems.Any())
                            addUpdateOrder.MenuItems = GetMenuItemResponse.MenuItems;

                        var GetDiningTableResponse = _getDiningTables(addUpdateOrder.RestaurantID).Result;

                        if (GetDiningTableResponse?.DiningTables != null && GetDiningTableResponse.DiningTables.Any())
                            addUpdateOrder.DiningTables = GetDiningTableResponse.DiningTables;
                    }
                }

            }

            return View(addUpdateOrder);
        }
        #endregion
        #region "Cascade dropdown"
        public async Task<IActionResult> GetMenuItems(int RestaurantId)
        {
            var menuItems = await _getMenuItems(RestaurantId);
            return Json(menuItems);
        }

        public async Task<IActionResult> GetDiningTables(int RestaurantId)
        {
            var diningTables = await _getDiningTables(RestaurantId);
            return Json(diningTables);
        }

        #endregion

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var addUpdateOrder = await GetExisingOrder(id);
            addUpdateOrder.IsDelete = true;
            var orderDeteleResponse = await _apiService.ExecuteRequest<RestaurantResponse>("Order/AddUpdateOrder", HttpMethod.Post, addUpdateOrder);

            return Json(orderDeteleResponse);
        }
        #region "Private functions"
        private async Task<MenuItemResponse> _getMenuItems(int RestaurantId)
        {
            return await _apiService.ExecuteRequest<MenuItemResponse>("MenuItem/GetMenuItem/" + RestaurantId, HttpMethod.Get, null);
        }

        private async Task<DiningTableResponse> _getDiningTables(int RestaurantId)
        {
            return await _apiService.ExecuteRequest<DiningTableResponse>("DiningTable/GetDiningTable/" + RestaurantId, HttpMethod.Get, null);
        }

        private async Task<AddUpdateOrder> GetExisingOrder(int OrderID)
        {
            AddUpdateOrder addUpdateOrder = new();
            var response = await _apiService.ExecuteRequest<OrderResponse>("Order/GetOrder/" + OrderID, HttpMethod.Get, null);

            if (response?.Orders != null && response.Orders.Any())
            {
                addUpdateOrder.OrderID = response.Orders[0].OrderID;
                addUpdateOrder.MenuItemID = response.Orders[0].MenuItemID;
                addUpdateOrder.DiningTableID = response.Orders[0].DiningTableID;
                addUpdateOrder.OrderDate = response.Orders[0].OrderDate;
                addUpdateOrder.ItemQuantity = response.Orders[0].ItemQuantity;
                addUpdateOrder.OrderAmount = response.Orders[0].OrderAmount;
                addUpdateOrder.RestaurantID = response.Orders[0].RestaurantID;

                var restaurants = await _apiService.ExecuteRequest<RestaurantResponse>("Restaurant/GetRestaurant/0", HttpMethod.Get, null);

                if (restaurants?.Restaurants != null && restaurants.Restaurants.Any())
                {
                    addUpdateOrder.Restaurants = restaurants.Restaurants;

                    var GetMenuItemResponse = _getMenuItems(addUpdateOrder.RestaurantID).Result;

                    if (GetMenuItemResponse?.MenuItems != null && GetMenuItemResponse.MenuItems.Any())
                        addUpdateOrder.MenuItems = GetMenuItemResponse.MenuItems;

                    var GetDiningTableResponse = _getDiningTables(addUpdateOrder.RestaurantID).Result;

                    if (GetDiningTableResponse?.DiningTables != null && GetDiningTableResponse.DiningTables.Any())
                        addUpdateOrder.DiningTables = GetDiningTableResponse.DiningTables;
                }
            }
            return addUpdateOrder;
        }
        #endregion
    }
}
