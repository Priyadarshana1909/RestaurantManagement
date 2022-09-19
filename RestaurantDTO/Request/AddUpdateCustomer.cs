using System.ComponentModel.DataAnnotations;
using RestaurantDTO.Response;

namespace RestaurantDTO.Request
{
    public class AddUpdateCustomer
    {
        public int CustomerId { get; set; }

        [Display(Name = "Restaurant Name")]
        [Required]
        public int RestaurantId { get; set; }

        public List<Restaurant> Restaurants { get; set; } = new();

        [Required(AllowEmptyStrings = false)]
        [MaxLength(50, ErrorMessage = "Customer name min length 50")]
        [MinLength(10, ErrorMessage = "Customer name min length 10")]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MinLength(10, ErrorMessage = "Mobile no min length 10")]
        [MaxLength(10, ErrorMessage = "Mobile no max length 10")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid mobile no")]
        [Display(Name = "Mobile No")]
        public string MobileNo { get; set; }

        public bool IsDelete { get; set; }
    }
}
