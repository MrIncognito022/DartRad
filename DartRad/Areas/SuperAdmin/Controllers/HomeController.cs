using DartRad.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DartRad.Areas.SuperAdmin.Controllers
{
    [Authorize(Roles = AppRoles.SuperAdmin)]
    [Area("SuperAdmin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
