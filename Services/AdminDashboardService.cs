using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkTrack.App.Data;
using WorkTrack.App.Dto;
using WorkTrack.App.Models;

namespace WorkTrack.App.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AdminDashboardService> _logger;

        public AdminDashboardService(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AdminDashboardService> logger)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<AdminDashboardVM> GetAdminDashboardData()
        {
            var data = new AdminDashboardVM();
            var allUsers = await _userManager.Users.ToListAsync();

            var employeeVms = new List<AdminUserVM>();
            var managerVms = new List<AdminUserVM>();
            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "Employee";
                var vm = new AdminUserVM
                {
                    Id = user.Id,
                    FullName = user.FullName ?? user.UserName ?? "",
                    Email = user.Email ?? "",
                    Phone = user.PhoneNumber ?? "",
                    Role = role
                };
                employeeVms.Add(vm);
                if (role == "Manager" || role == "Admin")
                    managerVms.Add(vm);
            }
            data.Employees = employeeVms;
            data.Managers = managerVms;

            // Get projects with members and manager details
            var projects = await _db.Projects
                .Include(p => p.Members)
                    .ThenInclude(pm => pm.User)
                .Include(p => p.Manager)
                .ToListAsync();

            data.Projects = projects.Select(p => new ProjectCard
            {
                Id = p.ProjectId,
                Name = p.ProjectName,
                Code = p.ProjectCode,
                Description = p.Description ?? string.Empty,
                DueDate = p.EndDate,
                Progress = p.Progress,
                Status = p.Status,
                ManagerName = p.Manager?.FullName ?? "Unassigned",
                TeamInitials = p.Members?
                    .Select(pm => GetInitials(pm.User.FullName ?? pm.User.UserName))
                    .Take(5)
                    .ToList() ?? new List<string>()
            }).ToList();

            return data;
        }

        public async Task<bool> ChangeUserRole(string userId, string role)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", userId);
                    return false;
                }

                // Remove from all current roles
                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                }

                // Ensure role exists
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }

                // Add to new role
                var result = await _userManager.AddToRoleAsync(user, role);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to add user to role: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing user role for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> CreateProject(Project project, List<string> memberIds)
        {
            try
            {
                // Validate manager exists and is a manager
                if (!string.IsNullOrEmpty(project.ManagerId))
                {
                    var manager = await _userManager.FindByIdAsync(project.ManagerId);
                    if (manager == null)
                    {
                        _logger.LogWarning("Manager not found: {ManagerId}", project.ManagerId);
                        return false;
                    }

                    var isManager = await _userManager.IsInRoleAsync(manager, "Manager");
                    if (!isManager)
                    {
                        _logger.LogWarning("User {ManagerId} is not a manager", project.ManagerId);
                        return false;
                    }
                }

                // Set creation date
                project.CreatedDate = DateTime.UtcNow;
                project.ModifiedDate = DateTime.UtcNow;

                // Add project
                _db.Projects.Add(project);
                await _db.SaveChangesAsync();

                // Add members
                if (memberIds != null && memberIds.Any())
                {
                    foreach (var userId in memberIds)
                    {
                        var user = await _userManager.FindByIdAsync(userId);
                        if (user != null)
                        {
                            _db.ProjectMembers.Add(new ProjectMember
                            {
                                ProjectId = project.ProjectId,
                                UserId = userId,
                                Role = "Member"
                            });
                        }
                    }
                    await _db.SaveChangesAsync();
                }

                _logger.LogInformation("Project created successfully: {ProjectId}", project.ProjectId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project: {ProjectName}", project.ProjectName);
                return false;
            }
        }

        public async Task<bool> UpdateProject(int projectId, ProjectStatus status, decimal progress)
        {
            try
            {
                var project = await _db.Projects.FindAsync(projectId);
                if (project == null)
                {
                    _logger.LogWarning("Project not found: {ProjectId}", projectId);
                    return false;
                }

                project.Status = status;
                project.Progress = Math.Clamp(progress, 0, 100);
                project.ModifiedDate = DateTime.UtcNow;

                // Auto-complete if progress is 100%
                if (progress >= 100)
                {
                    project.Status = ProjectStatus.Completed;
                    project.Progress = 100;
                }

                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project: {ProjectId}", projectId);
                return false;
            }
        }

        public async Task<bool> RemoveProjectMember(int projectId, string userId)
        {
            try
            {
                var member = await _db.ProjectMembers
                    .FirstOrDefaultAsync(m => m.ProjectId == projectId && m.UserId == userId);

                if (member == null)
                {
                    _logger.LogWarning("Project member not found: ProjectId={ProjectId}, UserId={UserId}",
                        projectId, userId);
                    return false;
                }

                _db.ProjectMembers.Remove(member);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing project member: ProjectId={ProjectId}, UserId={UserId}",
                    projectId, userId);
                return false;
            }
        }

        public async Task<bool> DeleteProject(int projectId)
        {
            try
            {
                var project = await _db.Projects
                    .Include(p => p.Members)
                    .FirstOrDefaultAsync(p => p.ProjectId == projectId);

                if (project == null)
                {
                    _logger.LogWarning("Project not found for deletion: {ProjectId}", projectId);
                    return false;
                }

                // Remove all members first
                if (project.Members != null && project.Members.Any())
                {
                    _db.ProjectMembers.RemoveRange(project.Members);
                }

                _db.Projects.Remove(project);
                await _db.SaveChangesAsync();

                _logger.LogInformation("Project deleted successfully: {ProjectId}", projectId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project: {ProjectId}", projectId);
                return false;
            }
        }

        public async Task<bool> AddOrUpdateEmployee(ApplicationUser user)
        {
            try
            {
                var existing = await _userManager.FindByIdAsync(user.Id);
                if (existing != null)
                {
                    // Update existing user
                    existing.FullName = user.FullName;
                    existing.Email = user.Email;
                    existing.UserName = user.Email;
                    existing.PhoneNumber = user.PhoneNumber;
                    existing.NormalizedEmail = user.Email.ToUpper();
                    existing.NormalizedUserName = user.Email.ToUpper();

                    var updateResult = await _userManager.UpdateAsync(existing);
                    if (!updateResult.Succeeded)
                    {
                        _logger.LogError("Failed to update employee: {Errors}",
                            string.Join(", ", updateResult.Errors.Select(e => e.Description)));
                    }
                    return updateResult.Succeeded;
                }
                else
                {
                    // Create new user
                    user.UserName = user.Email;
                    user.NormalizedEmail = user.Email.ToUpper();
                    user.NormalizedUserName = user.Email.ToUpper();
                    user.EmailConfirmed = true;

                    // TODO: In production, use a generated temporary password and require change on first login or send via email.
                    var result = await _userManager.CreateAsync(user, "DefaultPassword123!");
                    if (!result.Succeeded)
                    {
                        _logger.LogError("Failed to create employee: {Errors}",
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                        return false;
                    }

                    // Assign default Employee role
                    if (!await _roleManager.RoleExistsAsync("Employee"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Employee"));
                    }

                    await _userManager.AddToRoleAsync(user, "Employee");
                    _logger.LogInformation("Employee created successfully: {UserId}", user.Id);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding/updating employee: {Email}", user.Email);
                return false;
            }
        }

        public async Task<bool> AssignProjectManager(int projectId, string managerId)
        {
            try
            {
                var project = await _db.Projects.FindAsync(projectId);
                if (project == null)
                {
                    _logger.LogWarning("Project not found: {ProjectId}", projectId);
                    return false;
                }

                var manager = await _userManager.FindByIdAsync(managerId);
                if (manager == null)
                {
                    _logger.LogWarning("Manager not found: {ManagerId}", managerId);
                    return false;
                }

                // Verify user is a manager
                if (!await _userManager.IsInRoleAsync(manager, "Manager"))
                {
                    _logger.LogWarning("User is not a manager: {ManagerId}", managerId);
                    return false;
                }

                project.ManagerId = managerId;
                project.ModifiedDate = DateTime.UtcNow;

                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning project manager: ProjectId={ProjectId}, ManagerId={ManagerId}",
                    projectId, managerId);
                return false;
            }
        }

        private string GetInitials(string name)
        {
            if (string.IsNullOrEmpty(name)) return "??";
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return $"{parts[0][0]}{parts[1][0]}".ToUpper();
            return name.Length >= 2 ? name.Substring(0, 2).ToUpper() : name.ToUpper();
        }
    }
}