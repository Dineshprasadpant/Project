using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WorkTrack.App.Controllers
{
    [Authorize]
    public class HelpController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
