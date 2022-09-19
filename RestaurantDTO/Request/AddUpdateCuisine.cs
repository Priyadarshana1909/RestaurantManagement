using System.ComponentModel.DataAnnotations;
using RestaurantDTO.Response;

namespace RestaurantDTO.Request
{
    public class AddUpdateCuisine
    {
        public int CuisineId { get; set; }

        [Required]

        [Display(Name = "Restaurant Name")]
        public int RestaurantId { get; set; }

        public List<Restaurant> Restaurants { get; set; } = new();

        [Required(AllowEmptyStrings = false)]
        [MaxLength(50)]

        [Display(Name = "Cuisine Name")]
        public string CuisineName { get; set; }

        public bool IsDelete { get; set; }
    }
}
