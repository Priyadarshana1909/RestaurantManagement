using Microsoft.AspNetCore.Mvc;
using RestaurantBLL.Interface;
using RestaurantDTO.Request;
using RestaurantDTO.Response;

namespace RestaurantWebAPI.Controllers
{
    /// <summary>
    /// Bill controller - to get bill and add update bill
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class BillController : Controller
    {
        private readonly IManageBillBLL _manageBillBLL;

        /// <summary>
        /// Constructor for bill controller
        /// </summary>
        /// <param name="manageBillBLL"></param>
        public BillController(IManageBillBLL manageBillBLL)
        {
            _manageBillBLL = manageBillBLL;
        }


        #region Get Bill Details
        /// <summary>
        /// Get Bill details
        /// Pass 0 to get all bill details otherwise 
        /// it will give respective bill details
        /// In case of wrong bill id - it will give IsSuccessFull = false in response
        /// </summary>
        /// <param name="BillId"></param>
        /// <returns>BillResponse</returns>
        [HttpGet]
        [Route("GetBill/{BillId}")]
        public async Task<ActionResult<BillResponse>> GetBill(int BillId = 0)
        {
            var response = _manageBillBLL.GetBill(BillId > 0 ? BillId : null);
            return response;
        }
        #endregion

        #region Add / Update / Delete Bill
        /// <summary>
        /// To Add / update / delete bill
        /// To delete bill pass IsDelete flag to one in request
        /// </summary>
        /// <param name="addUpdateBill"></param>
        /// <returns>BaseResponse</returns>
        [HttpPost]
        [Route("AddUpdateBill")]
        public async Task<ActionResult<BaseResponse>> AddUpdateBill(AddUpdateBill addUpdateBill)
        {
            var response = _manageBillBLL.AddUpdateBill(addUpdateBill);
            return response;
        }
        #endregion

    }
}
