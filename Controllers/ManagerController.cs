using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkTrack.App.Data;

namespace WorkTrack.App.Controllers
{
public class ManagerController : Controller
{
    public readonly ApplicationDbContext _context;
    public ManagerController(ApplicationDbContext context)
    {
        _context = context;
    }
    /*
    - GetProjects() - JSON for projects
    - GetTeamMembers() - JSON for team
    - CreateTask(TaskViewModel model)
    - CreateProject(ProjectViewModel model)
    - GetGanttData() - JSON for timeline
    - GetStatistics() - JSON for stats cards
     */
    public IActionResult Dashboard()
    {
        return View();
    }
    /* [Authorize]
    public Task<IActionResult> GetProjects(int id)
     {

         return View();

     }*/
}
}

