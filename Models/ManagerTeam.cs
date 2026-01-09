
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkTrack.App.Models
{
    public class ManagerTeam
    {
        [Key]
        public int ManagerTeamId { get; set; }

        [Required]
        public string ManagerId { get; set; } = null!;

        [ForeignKey(nameof(ManagerId))]
        public ApplicationUser? Manager { get; set; }

        [Required]
        public string EmployeeId { get; set; } = null!;

        [ForeignKey(nameof(EmployeeId))]
        public ApplicationUser? Employee { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
    }
}

