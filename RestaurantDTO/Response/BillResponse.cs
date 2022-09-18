using System.ComponentModel.DataAnnotations;

namespace RestaurantDTO.Response
{
    public class BillResponse : BaseResponse
    {
        public List<Bill> Bills { get; set; } = new();
    }

    public class Bill
    {
       
        public int BillsID { get; set; }

        [Display(Name = "Order Id")]
        public int OrderID { get; set; }

        public int RestaurantID { get; set; }

        [Display(Name = "Bill Amount")]
        public double BillAmount { get; set; }

        public int CustomerID { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Restaurant Name")]
        public string RestaurantName { get; set; }
    }
}
