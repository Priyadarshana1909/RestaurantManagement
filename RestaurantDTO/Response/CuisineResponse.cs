using System.ComponentModel.DataAnnotations;

namespace RestaurantDTO.Response
{
    public class CuisineResponse : BaseResponse
    {
        public List<Cuisine> Cuisines { get; set; } = new();
    }

    public class Cuisine
    {
        public int CuisineID { get; set; }

        public int RestaurantID { get; set; }

        [Display(Name = "Restaurant Name")]
        public string RestaurantName { get; set; }

        [Display(Name = "Cuisine Name")]
        public string CuisineName { get; set; }
    }
}
