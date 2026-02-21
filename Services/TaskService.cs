using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkTrack.App.Data;
using WorkTrack.App.Dto;
using WorkTrack.App.Models;

namespace WorkTrack.App.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<TaskService> _logger;

        public TaskService(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            ILogger<TaskService> logger)
        {
            _db = db;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<(bool Success, string Message)> CreateTaskAsync(TaskCreateDto dto, string assignedById)
        {
            var project = await _db.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.ProjectId == dto.ProjectId);
            if (project == null)
                return (false, "Project not found.");

            var currentUser = await _userManager.FindByIdAsync(assignedById);
            if (currentUser == null)
                return (false, "User not found.");
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
            var isManager = await _userManager.IsInRoleAsync(currentUser, "Manager");
            if (!isAdmin && (project.ManagerId != assignedById || !isManager))
                return (false, "Only the project manager or an admin can create tasks for this project.");

            var assignee = await _userManager.FindByIdAsync(dto.AssignedToId);
            if (assignee == null)
                return (false, "Assignee not found.");
            var isMember = project.Members?.Any(m => m.UserId == dto.AssignedToId) ?? false;
            if (!isMember && project.ManagerId != dto.AssignedToId)
                return (false, "Assignee must be a member of the project.");

            var task = new TasksItem
            {
                TaskTitle = dto.TaskTitle,
                Description = dto.Description,
                ProjectId = dto.ProjectId,
                AssignedToId = dto.AssignedToId,
                AssignedById = assignedById,
                Priority = dto.Priority,
                Status = TasksStatus.Pending,
                DueDate = dto.DueDate,
                CreatedDate = DateTime.UtcNow
            };
            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Task {TaskId} created by {UserId} for project {ProjectId}", task.TasksId, assignedById, dto.ProjectId);
            return (true, "Task created successfully.");
        }
    }
}
