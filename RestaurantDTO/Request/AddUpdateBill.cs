using System.ComponentModel.DataAnnotations;
using RestaurantDTO.Response;

namespace RestaurantDTO.Request
{
    public class AddUpdateBill
    {
        public int BillsID { get; set; }

        [Required]
        [Display(Name = "Order Id")]
        public int? OrderID { get; set; }

        [Required]
        [Display(Name = "Restaurant Id")]
        public int? RestaurantID { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Bill Amount")]
        public double? BillAmount { get; set; }

        public int? CustomerID { get; set; }
        public List<Restaurant> Restaurants { get; set; } = new();

        public List<Customer> Customers { get; set; } = new();

        public List<Order> Orders { get; set; } = new();

        public bool IsDelete { get; set; }
        public List<Bill> Bills { get; set; } = new();
    }
}
