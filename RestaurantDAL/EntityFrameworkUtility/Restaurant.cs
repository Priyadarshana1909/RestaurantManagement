using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantDAL.EntityFrameworkUtility
{
    [Table("Restaurant")]
    public class Restaurant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RestaurantID { get; set; }

        [Required]
        [MaxLength(200)]
        public string RestaurantName { get; set; }

        [Required]
        [MaxLength(500)]
        public string Address { get; set; }

        [MaxLength(10)]
        public string MobileNo { get; set; }

        public List<Order> Orders { get; set; }

    }
}
