using Microsoft.AspNetCore.Mvc;
using RestaurantBLL.Interface;
using RestaurantDTO.Response;

namespace RestaurantWebAPI.Controllers
{
    /// <summary>
    /// Dining Table controller - to get dining table details
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class DiningTableController : Controller
    {
        private readonly IManageDiningTableBLL _manageDiningTableBLL;

        /// <summary>
        /// Constructor for dining table controller
        /// </summary>
        /// <param name="manageDiningTableBLL"></param>
        public DiningTableController(IManageDiningTableBLL manageDiningTableBLL)
        {
            _manageDiningTableBLL = manageDiningTableBLL;
        }

        #region "Get Dining Table Details
        /// <summary>
        /// Get dining table details
        /// it will give dining table details for respective restaurant id
        /// </summary>
        /// <param name="RestaurantId"></param>
        /// <returns>DiningTableResponse</returns>
        [HttpGet]
        [Route("GetDiningTable/{RestaurantId}")]
        public async Task<ActionResult<DiningTableResponse>> Get(int RestaurantId = 0)
        {
            var response = _manageDiningTableBLL.GetDiningTablesFromRestaurantId(RestaurantId);
            return response;
        }
        #endregion
    }
}
