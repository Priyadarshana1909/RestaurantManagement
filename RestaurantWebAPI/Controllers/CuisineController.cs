using Microsoft.AspNetCore.Mvc;

namespace RestaurantWebAPI.Controllers
{
    public class CuisineController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
