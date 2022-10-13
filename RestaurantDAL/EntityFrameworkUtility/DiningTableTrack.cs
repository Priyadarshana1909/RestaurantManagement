using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantDAL.EntityFrameworkUtility
{
    [Table("DiningTableTrack")]
    public class DiningTableTrack
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DiningTableTrackID { get; set; }

        [ForeignKey("DiningTableID")]
        public DiningTable DiningTable { get; set; }

        [Required]
        [MaxLength(100)]
        public string TableStatus { get; set; }
        

    }
}
