using WorkTrack.App.Dto;
using WorkTrack.App.Models;

public interface IAdminDashboardService
{
    Task<AdminDashboardVM> GetAdminDashboardData();

    Task<bool> ChangeUserRole(string userId, string role);

    Task<bool> CreateProject(Project project, List<string> memberIds);

    Task<bool> UpdateProject(int projectId, ProjectStatus status, decimal progress);

    Task<bool> AssignProjectManager(int projectId, string managerId);

    Task<bool> RemoveProjectMember(int projectId, string userId);

    Task<bool> DeleteProject(int projectId);
    Task<bool> AddOrUpdateEmployee(ApplicationUser user);
}
