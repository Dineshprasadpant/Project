using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkTrack.App.Models;
using WorkTrack.App.Services;

[Authorize(Roles = "Employee")]
public class EmployeeController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmployeeDashboardService _dashboardService;

    public EmployeeController(UserManager<ApplicationUser> userManager, IEmployeeDashboardService dashboardService)
    {
        _userManager = userManager;
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var userId = _userManager.GetUserId(User);
        var vm = await _dashboardService.GetDashboardData(userId);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateTaskStatus(int taskId, TasksStatus status)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
        var success = await _dashboardService.UpdateTaskStatus(taskId, status, userId);
        if (!success)
            return NotFound();
        return RedirectToAction(nameof(Dashboard));
    }
}