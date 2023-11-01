using DartRad.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DartRad.Areas.ContentCreator.Controllers
{
    [Area("ContentCreator")]
    [Authorize(Roles = AppRoles.ContentCreator)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
