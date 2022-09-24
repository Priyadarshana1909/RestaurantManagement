using System.ComponentModel.DataAnnotations;

namespace RestaurantDTO.Response
{
    public class OrderResponse : BaseResponse
    {
        public List<Order> Orders { get; set; } = new();
    }

    public class Order
    {
        public int OrderID { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        public int RestaurantID { get; set; }

        public int MenuItemID { get; set; }

        [Display(Name = "Quantity")]
        public int ItemQuantity { get; set; }

        [Display(Name = "Price")]
        public double ItemPrice { get; set; }

        [Display(Name = "Order Amount")]
        public double? OrderAmount { get; set; }

        public int DiningTableID { get; set; }

        [Display(Name = "Restaurant Name")]
        public string RestaurantName { get; set; }

        [Display(Name = "Item")]
        public string ItemName { get; set; }

    }
}
