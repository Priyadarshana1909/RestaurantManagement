using Microsoft.AspNetCore.Mvc;
using RestaurantBLL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantWebAPI.Controllers
{
    /// <summary>
    /// Cuisine controller - to get cuisine and add update cuisine
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class CuisineController : Controller
    {
        private readonly IManageCuisineBLL _manageCuisineBLL;

        /// <summary>
        /// Constructor for cuisine controller
        /// </summary>
        /// <param name="manageCuisineBLL"></param>
        public CuisineController(IManageCuisineBLL manageCuisineBLL)
        {
            _manageCuisineBLL = manageCuisineBLL;
        }

        #region Get Cuisine Details
        /// <summary>
        /// Get cuisine details
        /// Pass 0 to get all cuisine details otherwise it will give respective cuisine details
        /// In case of wrong cuisine id - it will give IsSuccessFull = false in response
        /// </summary>
        /// <param name="CuisineId"></param>
        /// <returns>CuisineResponse</returns>
        [HttpGet]
        [Route("GetCuisine/{CuisineId}")]
        public async Task<ActionResult<CuisineResponse>> Get(int CuisineId = 0)
        {
            var response = _manageCuisineBLL.GetCuisines(CuisineId > 0? CuisineId : null);
            return response;
        }
        #endregion

        #region "Add / Update / Delete Cuisine"
        /// <summary>
        /// To Add / update / delete cuisine
        /// To delete cuisine pass IsDelete flag to one in request
        /// </summary>
        /// <param name="addUpdateCuisine"></param>
        /// <returns>BaseResponse</returns>
        [HttpPost]
        [Route("AddUpdateCuisine")]
        public async Task<ActionResult<BaseResponse>> AddUpdateCuisine(AddUpdateCuisine addUpdateCuisine)
        {
            var response = _manageCuisineBLL.AddUpdateCuisine(addUpdateCuisine);
            return response;
        }
        #endregion
    }
}
