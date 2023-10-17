using Microsoft.AspNetCore.Mvc;

namespace testAppDynamicSun.Controllers
{
    public class LoadController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
