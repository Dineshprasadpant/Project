using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkTrack.App.Models
{
    public class Timeline
    {
        [Key]
        public int TimelineId { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project? Project { get; set; }

        [Required]
        [StringLength(100)]
        public string Phase { get; set; } = null!;  // e.g., Design, Development, Testing

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [Range(0, 100)]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Progress { get; set; } = 0;

        [StringLength(20)]
        public string? Color { get; set; }  // e.g., "#FF0000" for red
    }
}
