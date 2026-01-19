using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkTrack.App.Data;
using WorkTrack.App.Dto;
using WorkTrack.App.Models;

namespace WorkTrack.App.Services
{
    public class ManagerDashboardService : IManagerDashboardService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ManagerDashboardService(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<ManagerDashboardVM> GetDashboardData(string userId)
        {
            var projects = await _db.Projects
                .Where(p => p.ManagerId == userId)
                .Include(p => p.Members)
                    .ThenInclude(pm => pm.User)
                .Include(p => p.Tasks)
                .ToListAsync();

            var allTasks = projects.SelectMany(p => p.Tasks).ToList();

            var vm = new ManagerDashboardVM
            {
                OngoingProjects = projects.Count(p => p.Status == ProjectStatus.Active),
                TeamMembers = projects
                    .SelectMany(p => p.Members)
                    .Select(pm => pm.UserId)
                    .Distinct()
                    .Count(),
                ActiveTasks = allTasks.Count(t => t.Status != TasksStatus.Completed),
                PendingTasks = allTasks.Count(t => t.Status == TasksStatus.Pending),
                CompletionRate = allTasks.Any()
                    ? Math.Round(allTasks.Count(t => t.Status == TasksStatus.Completed) * 100.0 / allTasks.Count, 2)
                    : 0,

                Projects = projects.Select(p => new ProjectCardVM
                {
                    ProjectId = p.ProjectId,
                    Name = p.ProjectName,
                    Code = p.ProjectCode,
                    Status = p.Status.ToString(),
                    MembersCount = p.Members.Count,
                    EndDate = p.EndDate,
                    Progress = p.Progress
                }).ToList(),

                TeamMembersList = projects
                    .SelectMany(p => p.Members)
                    .GroupBy(pm => pm.User)
                    .Select(g => new TeamMemberVM
                    {
                        Id = g.Key.Id,
                        FullName = g.Key.FullName,
                        Email = g.Key.Email,
                        AssignedDate = g.Min(pm => pm.AssignedDate),
                        TaskCount = allTasks.Count(t => t.AssignedToId == g.Key.Id),
                    })
                    .ToList()
            };

            return vm;
        }
    }
}
