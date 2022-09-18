using Microsoft.AspNetCore.Mvc;
using RestaurantBLL.Interface;
using RestaurantDTO.Request;
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


        [HttpPost]
        [Route("AddUpdateBill")]
        public async Task<ActionResult<BaseResponse>> AddUpdateBill(AddUpdateBill addUpdateBill)
        {
            var response = _manageBillBLL.AddUpdateBill(addUpdateBill);
            return response;
        }
       
    }
}
