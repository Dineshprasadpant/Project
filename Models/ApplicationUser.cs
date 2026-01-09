using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkTrack.App.Models
{
    public enum UserRole
    {
        Admin,
        Manager,
        Employee
    }   
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(150)]
        public string FullName { get; set; } = null!;

        [Required]
        public UserRole Role { get; set; }

        public string? ManagerId { get; set; }

        [ForeignKey(nameof(ManagerId))]
        public ApplicationUser? Manager { get; set; }

        public ICollection<ApplicationUser> Subordinates { get; set; }
            = new List<ApplicationUser>();

        [StringLength(100)]
        public string? Department { get; set; }

        [Required]
        [StringLength(100)]
        public string Position { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
