using Microsoft.AspNetCore.Mvc;
using RestaurantBLL.Interface;
using RestaurantDTO.Response;

namespace RestaurantWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BillController : Controller
    {
        private readonly IManageBillBLL _manageBillBLL;

        public BillController(IManageBillBLL manageBillBLL)
        {
            _manageBillBLL = manageBillBLL;
        }

        [HttpGet]
        [Route("GetBill/{BillId}")]
        public async Task<ActionResult<BillResponse>> GetBill(int BillId = 0)
        {
            var response = _manageBillBLL.GetBill(BillId > 0 ? BillId : null);
            return response;
        }

        //[HttpPost]
        //[Route("AddUpdateOrder")]
        //public async Task<ActionResult<BaseResponse>> AddUpdateOrder(AddUpdateOrder AddUpdateOrder)
        //{
        //    var response = _manageOrderBLL.AddUpdateOrder(AddUpdateOrder);
        //    return response;
        //}
    }
}
