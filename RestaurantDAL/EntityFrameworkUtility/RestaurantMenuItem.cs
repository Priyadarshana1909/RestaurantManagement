using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantDAL.EntityFrameworkUtility
{
    [Table("RestaurantMenuItem")]
    public class RestaurantMenuItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MenuItemID { get; set; }

        public int CuisineID { get; set; }

        public string ItemName { get; set; }

        public double ItemPrice { get; set; }

        public List<Order> Orders { get; set; }
    }
}
