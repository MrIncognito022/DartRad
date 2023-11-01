using DartRad.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DartRad.Areas.Editor.Controllers
{
    [Area("Editor")]
    [Authorize(Roles = AppRoles.Editor)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
