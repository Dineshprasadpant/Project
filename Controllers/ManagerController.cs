using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkTrack.App.Dto;
using WorkTrack.App.Models;
using WorkTrack.App.Services;

namespace WorkTrack.App.Controllers
{
    [Authorize(Roles = "Manager,Admin")]
    public class ManagerController : Controller
    {
        private readonly IManagerDashboardService _dashboardService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITaskService _taskService;

        public ManagerController(
            IManagerDashboardService dashboardService,
            UserManager<ApplicationUser> userManager,
            ITaskService taskService)
        {
            _dashboardService = dashboardService;
            _userManager = userManager;
            _taskService = taskService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);
            var vm = await _dashboardService.GetDashboardData(userId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTask(TaskCreateDto dto)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var (success, message) = await _taskService.CreateTaskAsync(dto, userId);
            if (success)
                TempData["SuccessMessage"] = message;
            else
                TempData["ErrorMessage"] = message;
            return RedirectToAction(nameof(Dashboard));
        }
    }
}
