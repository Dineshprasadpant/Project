using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkTrack.App.Models
{
    public class ProjectMember
    {
        [Key]
        public int ProjectMemberId { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project? Project { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = null!; // Lead, Developer, Designer, QA, etc.

        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
    }
}
