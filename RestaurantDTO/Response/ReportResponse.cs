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
        public int? RestaurantID { get; set; }
        public int? DiningTableID { get; set; }
        public int? OrderID { get; set; }
        public string? Location { get; set; }

        public int? ItemQuantity { get; set; }
       
        public double? OrderAmount { get; set; }
      
        [Display(Name = "Order Date")]
        public DateTime? OrderDate { get; set; }
       

    }
}
