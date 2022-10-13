using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantDAL.EntityFrameworkUtility
{
    [Table("DiningTable")]
    public class DiningTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DiningTableID { get; set; }

        [ForeignKey("RestaurantID")]
        public Restaurant Restaurant { get; set; }

        [Required]
        [MaxLength(100)]
        public string Location { get; set; }

        public DiningTableTrack DiningTableTrack { get; set; }


    }
}
