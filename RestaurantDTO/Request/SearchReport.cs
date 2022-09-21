using System.ComponentModel.DataAnnotations;
using RestaurantDTO.Response;

namespace RestaurantDTO.Request
{
  
    public class SearchReport
    {
        [Display(Name = "Customer ID")]        
        public int? CustomerID { get; set; }

        [Display(Name = "Customer Name")]
        public string? CustomerName { get; set; }    
        public List<Customer>? Customers { get; set; } = new();

        [Display(Name = "Restaurant Name")]
        public int? RestaurantID { get; set; }
        public List<Restaurant>? Restaurants { get; set; } = new();
        public int? DiningTableID { get; set; }

        public List<DiningTable>? DiningTables { get; set; } = new();
        public int? OrderID { get; set; }

        public List<Order>? Orders { get; set; } = new();
        public string? Location { get; set; }

        public int ItemQuantity1 { get; set; }

        public int ItemQuantity2 { get; set; }

        public float OrderAmount1 { get; set; }

        public float OrderAmount2 { get; set; }

        public DateTime? OrderDate { get; set; }

        public DateTime? OrderDateFrom { get; set; }

        public DateTime? OrderDateTo { get; set; }

        public string? SortBy { get; set; }
    }
}
