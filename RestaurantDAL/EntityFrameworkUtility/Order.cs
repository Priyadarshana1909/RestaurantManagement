using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantDAL.EntityFrameworkUtility
{
    [Table("Order")]
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID { get; set; }

        public DateTime OrderDate { get; set; }

        [ForeignKey("RestaurantID")]
        public Restaurant Restaurant { get; set; }

        [ForeignKey("MenuItemID")]
        public RestaurantMenuItem RestaurantMenuItem { get; set; }

        public int ItemQuantity { get; set; }

        public double OrderAmount { get; set; }

        public int DiningTableID { get; set; }
        
    }
}
