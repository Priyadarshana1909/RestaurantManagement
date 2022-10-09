using System.ComponentModel.DataAnnotations;

namespace RestaurantDTO.Response
{
    public class ReportResponse : BaseResponse
    {
        public List<CustomerReport> CustomerReports { get; set; } = new();
    }

    public class CustomerReport
    {
        public int? CustomerID { get; set; }

        [Display(Name = "Customer Name")]
        public string? CustomerName { get; set; }

        [Display(Name = "Restaurant Name")]
        public string? RestaurantName { get; set; }

        [Display(Name = "Order Id")]
        public int? OrderID { get; set; }

        [Display(Name = "Dining Table Location")]
        public string? Location { get; set; }

        [Display(Name = "Quantity")]
        public int? ItemQuantity { get; set; }

        [Display(Name = "Amount")]
        public double? OrderAmount { get; set; }
      
        [Display(Name = "Order Date")]
        public DateTime? OrderDate { get; set; }
    }
}
