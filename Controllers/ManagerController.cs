using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkTrack.App.Models;
using WorkTrack.App.Services;

[Authorize(Roles ="Manager")]
public class ManagerController : Controller
{
    private readonly IManagerDashboardService _dashboardService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ManagerController(IManagerDashboardService dashboardService, UserManager<ApplicationUser> userManager)
    {
        _dashboardService = dashboardService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Dashboard()
    {
        var userId = _userManager.GetUserId(User);
        var vm = await _dashboardService.GetDashboardData(userId);
        return View(vm);
    }

}
