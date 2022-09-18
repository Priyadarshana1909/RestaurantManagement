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
        public int? RestaurantId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Bill Amount")]
        public double? BillAmount { get; set; }

        public List<Restaurant> Restaurants { get; set; } = new();

        public List<Customer> Customers { get; set; } = new();

        public bool IsDelete { get; set; }
    }
}
