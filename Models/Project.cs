using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkTrack.App.Models
{
    public enum ProjectStatus
    {
        Active,
        AtRisk,
        Completed,
        OnHold
    }
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }

        [Required]
        [StringLength(50)]
        public string ProjectCode { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string ProjectName { get; set; } = null!;

        [Column(TypeName = "text")]
        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public ProjectStatus Status { get; set; }

        [Range(0, 100)]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Progress { get; set; }

        [Required]
        public string ManagerId { get; set; } = null!;

        [ForeignKey(nameof(ManagerId))]
        public ApplicationUser? Manager { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }
    }
}
