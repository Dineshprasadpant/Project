using Microsoft.Build.Utilities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkTrack.App.Models
{
    public class TaskHistory
    {
        [Key]
        public int TaskHistoryId { get; set; }

        [Required]
        public int TasksId { get; set; }

        [ForeignKey(nameof(TasksId))]
        public TasksItem? Task { get; set; }

        [Required]
        public string ChangedById { get; set; } = null!;

        [ForeignKey(nameof(ChangedById))]
        public ApplicationUser? ChangedBy { get; set; }

        [Required]
        [StringLength(50)]
        public string ChangeType { get; set; } = null!; // Status Change, Assignment, Priority Change

        [StringLength(100)]
        public string? OldValue { get; set; }

        [StringLength(100)]
        public string? NewValue { get; set; }

        public DateTime ChangeDate { get; set; } = DateTime.UtcNow;
    }
}
