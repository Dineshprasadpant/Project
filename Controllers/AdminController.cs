using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkTrack.App.Dto;
using WorkTrack.App.Models;
using WorkTrack.App.Services;

namespace WorkTrack.App.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]/[action]")]
    public class AdminController : Controller
    {
        private readonly IAdminDashboardService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminDashboardService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var model = await _adminService.GetAdminDashboardData();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin dashboard");
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeRole([FromBody] ChangeRoleDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid data provided" });
                }

                var result = await _adminService.ChangeUserRole(dto.UserId, dto.Role);
                return Ok(new
                {
                    success = result,
                    message = result ? "Role changed successfully" : "Failed to change role. Please check logs."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing role for user {UserId}", dto.UserId);
                return StatusCode(500, new { success = false, message = "An error occurred while changing role" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
        {
            try
            {
                _logger.LogInformation("Attempting to create project: {ProjectName}", dto.ProjectName);

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    _logger.LogWarning("Invalid model state: {Errors}", string.Join(", ", errors));
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid data: " + string.Join(", ", errors)
                    });
                }

                // Validate end date
                if (dto.EndDate <= DateTime.UtcNow)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "End date must be in the future"
                    });
                }

                var project = new Project
                {
                    ProjectName = dto.ProjectName,
                    ProjectCode = dto.ProjectCode,
                    Description = dto.Description,
                    StartDate = DateTime.UtcNow,
                    EndDate = dto.EndDate,
                    Status = ProjectStatus.Active,
                    Progress = 0,
                    ManagerId = dto.ManagerId,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow
                };

                var result = await _adminService.CreateProject(project, dto.MemberIds ?? new List<string>());

                if (result)
                {
                    _logger.LogInformation("Project created successfully: {ProjectName}", dto.ProjectName);
                }
                else
                {
                    _logger.LogWarning("Failed to create project: {ProjectName}", dto.ProjectName);
                }

                return Ok(new
                {
                    success = result,
                    message = result ? "Project created successfully" : "Failed to create project. Check if the manager exists and has Manager role."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project: {ProjectName}", dto.ProjectName);
                return StatusCode(500, new
                {
                    success = false,
                    message = $"An error occurred: {ex.Message}"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProject([FromBody] UpdateProjectDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid data provided" });
                }

                var result = await _adminService.UpdateProject(dto.ProjectId, dto.Status, dto.Progress);
                return Ok(new
                {
                    success = result,
                    message = result ? "Project updated successfully" : "Failed to update project"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project {ProjectId}", dto.ProjectId);
                return StatusCode(500, new { success = false, message = "An error occurred while updating project" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignManager([FromBody] AssignManagerDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid data provided" });
                }

                var result = await _adminService.AssignProjectManager(dto.ProjectId, dto.ManagerId);
                return Ok(new
                {
                    success = result,
                    message = result ? "Manager assigned successfully" : "Failed to assign manager. User must have Manager role."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning manager to project {ProjectId}", dto.ProjectId);
                return StatusCode(500, new { success = false, message = "An error occurred while assigning manager" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveMember([FromBody] RemoveMemberDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid data provided" });
                }

                var result = await _adminService.RemoveProjectMember(dto.ProjectId, dto.UserId);
                return Ok(new
                {
                    success = result,
                    message = result ? "Member removed successfully" : "Failed to remove member"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing member from project {ProjectId}", dto.ProjectId);
                return StatusCode(500, new { success = false, message = "An error occurred while removing member" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProject([FromBody] DeleteProjectDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid data provided" });
                }

                var result = await _adminService.DeleteProject(dto.ProjectId);
                return Ok(new
                {
                    success = result,
                    message = result ? "Project deleted successfully" : "Failed to delete project"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project {ProjectId}", dto.ProjectId);
                return StatusCode(500, new { success = false, message = "An error occurred while deleting project" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateEmployee([FromBody] UpsertEmployeeDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid data: " + string.Join(", ", errors)
                    });
                }

                var user = new ApplicationUser
                {
                    Id = dto.Id ?? Guid.NewGuid().ToString(),
                    FullName = dto.FullName,
                    Email = dto.Email,
                    UserName = dto.Email,
                    PhoneNumber = dto.PhoneNumber
                };

                var result = await _adminService.AddOrUpdateEmployee(user);
                return Ok(new
                {
                    success = result,
                    message = result ? "Employee saved successfully" : "Failed to save employee. Check if email is unique."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding/updating employee: {Email}", dto.Email);
                return StatusCode(500, new
                {
                    success = false,
                    message = $"An error occurred: {ex.Message}"
                });
            }
        }
    }

    // Additional DTOs for the new endpoints
    
}