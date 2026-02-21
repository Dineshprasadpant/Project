using System.ComponentModel.DataAnnotations;
using WorkTrack.App.Models;

namespace WorkTrack.App.Dto
{
    public class TaskCreateDto
    {
        public int ProjectId { get; set; }
        public string AssignedToId { get; set; } = null!;
        [Required, StringLength(200)]
        public string TaskTitle { get; set; } = null!;
        public string? Description { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    }
}
