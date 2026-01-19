using System;
using System.Collections.Generic;
using WorkTrack.App.Models;

namespace WorkTrack.App.Dto
{
    public class AdminUserVM
    {
        public string Id { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
    public class AdminDashboardVM
    {
        public List<ApplicationUser> Employees { get; set; } = new();
        public List<ApplicationUser> Managers { get; set; } = new();
        public List<ProjectCard> Projects { get; set; } = new();
    }

    public class ProjectCard
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public decimal Progress { get; set; }
        public ProjectStatus Status { get; set; }
        public List<string> TeamInitials { get; set; } = new List<string>();
        public string ManagerName { get; set; } = string.Empty;
    }

    public class ChangeRoleDto
    {
        public string UserId { get; set; } = null!;
        public string Role { get; set; } = null!;
    }

    public class CreateProjectDto
    {
        public string ProjectName { get; set; } = null!;
        public string ProjectCode { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime EndDate { get; set; }
        public string ManagerId { get; set; } = null!;
        public List<string> MemberIds { get; set; } = new();
    }

    public class UpsertEmployeeDto
    {
        public string? Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
    }
    public class UpdateProjectDto
    {
        public int ProjectId { get; set; }
        public ProjectStatus Status { get; set; }
        public decimal Progress { get; set; }
    }

    public class AssignManagerDto
    {
        public int ProjectId { get; set; }
        public string ManagerId { get; set; } = null!;
    }

    public class RemoveMemberDto
    {
        public int ProjectId { get; set; }
        public string UserId { get; set; } = null!;
    }

    public class DeleteProjectDto
    {
        public int ProjectId { get; set; }
    }
}