using WorkTrack.App.Dto;
using WorkTrack.App.Models;

namespace WorkTrack.App.Services
{
    public interface IEmployeeDashboardService
    {
        Task<EmployeeDashboardVM> GetDashboardData(string userId);
        Task<bool> UpdateTaskStatus(int taskId, TasksStatus status, string userId);
    }
}
