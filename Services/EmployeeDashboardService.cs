using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkTrack.App.Data;
using WorkTrack.App.Dto;
using WorkTrack.App.Models;
using WorkTrack.App.Dto;
namespace WorkTrack.App.Services
{
    public class EmployeeDashboardService : IEmployeeDashboardService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeeDashboardService(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<EmployeeDashboardVM> GetDashboardData(string userId)
        {
            var data = new EmployeeDashboardVM();

            var projects = await _db.ProjectMembers
                .Where(pm => pm.UserId == userId)
                .Include(pm => pm.Project)
                    .ThenInclude(p => p.Manager) 
                .Select(pm => pm.Project)
                .ToListAsync();

            data.Projects = projects.Select(p => new EmployeeProjectVM
            {
                ProjectId = p.ProjectId,
                ProjectName = p.ProjectName,
                ProjectCode = p.ProjectCode,
                ManagerName = p.Manager.FullName ?? p.Manager.UserName,
                EndDate = p.EndDate,
                Progress = p.Progress
            }).ToList();
            
            var tasks = await _db.Tasks
                .Where(t => t.AssignedToId == userId)
                .Include(t => t.Project)
                    .ThenInclude(p => p.Manager)
                .ToListAsync();

            data.Tasks = tasks.Select(t => new EmployeeTaskVM
            {
                TaskId = t.TasksId,
                TaskTitle = t.TaskTitle,
                Description = t.Description,
                ProjectName = t.Project.ProjectName,
                ManagerName = t.Project.Manager.FullName ?? t.Project.Manager.UserName,
                Status = t.Status,
                DueDate = t.DueDate
            }).ToList();

            return data;
        }

        public async Task<bool> UpdateTaskStatus(int taskId, TasksStatus status)
        {
            var task = await _db.Tasks.FindAsync(taskId);
            if (task == null) return false;

            task.Status = status;

            if (status == TasksStatus.Completed)
                task.CompletedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
