using DartRad.Areas.Editor.Models;
using DartRad.Data;
using DartRad.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace DartRad.Areas.Editor.Controllers
{
    [Area("Editor")]
    [Authorize(Roles = AppRoles.Editor)]
    [Route("[Area]/[Controller]")]
    public class SubscribersController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IMailService _mailService;

        public SubscribersController(AppDbContext dbContext, IMailService mailService)
        {
            this._dbContext = dbContext;
            this._mailService = mailService;
        }
        public IActionResult Index()
        {
           return View();
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(Subscriber model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var subscriberWithSameEmail = await _dbContext.Subscriber.FirstOrDefaultAsync(x => x.Email == model.Email);
            if (subscriberWithSameEmail != null)
            {
                ModelState.AddModelError("Email", "Provided Email address is already subscribed");
                return View(model);
            }
            model.CreatedAt = DateTime.Now;
            model.CreatedBy = GetUserId();

            _dbContext.Subscriber.Add(model);
            await _dbContext.SaveChangesAsync();

            TempData[TempDataKeys.SuccessMessage] = "Subscriber added successfully.";

            return RedirectToAction(nameof(Index));

        }


        [HttpGet("Edit")]
        public async Task< IActionResult> Edit(int id)
        {
            var subcriberById = await _dbContext.Subscriber.FirstOrDefaultAsync(x => x.Id == id);
            if(subcriberById == null)
            {
                return NotFound();
            }

            return View(subcriberById);
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(Subscriber model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var subcriberFromDb = await _dbContext.Subscriber.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (subcriberFromDb == null)
            {
                return NotFound();
            }

            var subscriberWithSameEmail = await _dbContext.Subscriber.AsNoTracking().FirstOrDefaultAsync(x => x.Email == model.Email && x.Id != model.Id);
            if (subscriberWithSameEmail != null)
            {
                ModelState.AddModelError("Email", "Provided Email address is already subscribed");
                return View(model);
            }

            subcriberFromDb.FirstName = model.FirstName;
            subcriberFromDb.LastName = model.LastName;
            subcriberFromDb.Email = model.Email;

            _dbContext.Subscriber.Update(subcriberFromDb);
            await _dbContext.SaveChangesAsync();

            TempData[TempDataKeys.SuccessMessage] = "Subscriber updated successfully.";

            return RedirectToAction(nameof(Index));

        }

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            // Get the cc by id
            var subscriberFromDb = await _dbContext.Subscriber.FindAsync(id);

            // Check if cc exists
            if (subscriberFromDb == null)
            {
                return NotFound();
            }

            // Remove the cc from the database
            _dbContext.Subscriber.Remove(subscriberFromDb);
            await _dbContext.SaveChangesAsync();

            // Redirect to the index page with success message
            TempData[TempDataKeys.SuccessMessage] = "Subcriber deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }


        #region Ajax

        [HttpPost("AjaxSubscribers")]
        public JsonResult AjaxSubscribers([FromBody] SubscribersListRequest request)
        {
            // get all content creators added by logged in user
            var query =  _dbContext.Subscriber.Include(x => x.CreatedByEditor).AsQueryable();

            int totalRecords = query.Count();

            query = query.Skip(request.PageNumber * request.PageSize).Take(request.PageSize);

            return Json(new PaginatedAjaxResponse<Subscriber>
            {
                Data = query.ToList(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalRecords = totalRecords
            });
        }
        #endregion
    }
}
