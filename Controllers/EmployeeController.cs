using Microsoft.AspNetCore.Mvc;

namespace WorkTrack.App.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
