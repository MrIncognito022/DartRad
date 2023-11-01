using DartRad.Data;
using DartRad.Utilities;
using DartRad.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace DartRad.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, AppDbContext dbContext)
        {
            _logger = logger;
            this._dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View("Landing");
        }

        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Subscribe()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(Subscriber model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var subscriberWithSameEmail = await _dbContext.Subscriber.FirstOrDefaultAsync(x => x.Email == model.Email);
            if(subscriberWithSameEmail != null)
            {
                ModelState.AddModelError("Email", "Provided Email address is already subscribed");
                return View(model);
            }
            model.CreatedAt = DateTime.Now;

            _dbContext.Subscriber.Add(model);
            await _dbContext.SaveChangesAsync();
            ViewBag.Subscribed = true;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }
}