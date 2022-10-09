using System.ComponentModel.DataAnnotations;

namespace RestaurantDTO.Request
{

    public class SearchReport
    {

        [Display(Name = "Customer Name")]
        public string? CustomerName { get; set; }

        [Display(Name = "Restaurant Name")]
        public string? RestaurantName { get; set; }

        public int? OrderID { get; set; }

        public string? Location { get; set; }

        public int ItemQuantity1 { get; set; }

        public int ItemQuantity2 { get; set; }

        public float OrderAmount1 { get; set; }

        public float OrderAmount2 { get; set; }
        
        public DateTime? OrderDateFrom { get; set; }

        public DateTime? OrderDateTo { get; set; }

        public string? SortBy { get; set; }

        public string? SortDirection { get; set; }
    }
}
