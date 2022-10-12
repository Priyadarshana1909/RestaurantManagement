using RestaurantDTO.Response;
using System.ComponentModel.DataAnnotations;

namespace RestaurantDTO.Request
{
    public class AddUpdateOrder
    {
        public int OrderID { get; set; }

        [Display(Name = "Order Date")]
        public DateTime? OrderDate { get; set; }

        [Display(Name = "Restaurant Name")]
        public int RestaurantID { get; set; }

        [Display(Name = "Item")]
        public int MenuItemID { get; set; }

        [Display(Name = "Price")]
        public double ItemPrice { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Quantity")]
        public int? ItemQuantity { get; set; }

        public double? OrderAmount { get; set; }

        [Display(Name = "Dining Table")]
        public int DiningTableID { get; set; }

        public List<Restaurant> Restaurants { get; set; } = new();

        public List<DiningTable> DiningTables { get; set; } = new();

        public List<MenuItem> MenuItems { get; set; } = new();

        public bool IsDelete { get; set; }
       
    }
}
