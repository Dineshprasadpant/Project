using Microsoft.AspNetCore.Mvc;

namespace WorkTrack.App.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
