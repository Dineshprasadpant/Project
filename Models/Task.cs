using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkTrack.App.Models
{
    public enum TaskPriority
    {
        Low,
        Medium,
        High,
        Critical
    }
    public class TasksItem
    {
        [Key]
        public int TasksId { get; set; }

        [Required]
        [StringLength(200)]
        public string TaskTitle { get; set; } = null!;

        [Column(TypeName = "text")]
        public string? Description { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project? Project { get; set; }

        [Required]
        public string AssignedToId { get; set; } = null!;

        [ForeignKey(nameof(AssignedToId))]
        public ApplicationUser? AssignedTo { get; set; }

        [Required]
        public string AssignedById { get; set; } = null!;

        [ForeignKey(nameof(AssignedById))]
        public ApplicationUser? AssignedBy { get; set; }

        [Required]
        public TaskPriority Priority { get; set; }

        [Required]
        public TaskStatus Status { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }
    }
}
