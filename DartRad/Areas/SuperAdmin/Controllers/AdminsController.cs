using AutoMapper;
using DartRad.Areas.Editor.Models;
using DartRad.Areas.SuperAdmin.Models;
using DartRad.Data;
using DartRad.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace DartRad.Areas.SuperAdmin.Controllers
{
    [Authorize(Roles = AppRoles.SuperAdmin)]
    [Area("SuperAdmin")]
    [Route("[Area]/[Controller]")]
    public class AdminsController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public AdminsController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
       
        [HttpGet("Index")]
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
        public async Task<IActionResult> Create(AdminCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // get logged in user id
            var userId = this.GetUserId();

            // check if email already exists
            var emailExists = await _dbContext.Editor.AnyAsync(a => a.Email == model.Email);
            if (emailExists)
            {
                ModelState.AddModelError(nameof(AdminCreateViewModel.Email), "Email already exists");
                return View(model);
            }

            emailExists = await _dbContext.ContentCreator.AnyAsync(a => a.Email == model.Email);
            if (emailExists)
            {
                ModelState.AddModelError(nameof(AdminCreateViewModel.Email), "Email already exists");
                return View(model);
            }

            // map viewmodel to entity
            var admin = _mapper.Map<Entities.Editor>(model);
            admin.CreatedBy = userId;

            // hash password
            var passwordHash = AuthHelper.HashPassword(model.Password);
            admin.Password = passwordHash;

            admin.CreatedAt = DateTime.Now;
            // add to database
            _dbContext.Editor.Add(admin);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var adminFromDb = await _dbContext.Editor.FindAsync(id);

            // check if admin exists
            if (adminFromDb == null)
            {
                return NotFound();
            }

            var adminViewModel = _mapper.Map<AdminUpdateViewModel>(adminFromDb);
            return View(adminViewModel);
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(int id, AdminUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get the admin from the database
            var admin = await _dbContext.Editor.FindAsync(id);

            if (admin == null)
            {
                return NotFound();
            }

            // Update the admin with the new data
            admin.Name = model.Name;
            admin.Email = model.Email;

            // recheck for email, it shouldn't exist in admin/ cc

            if (await _dbContext.Editor.AnyAsync(x => x.Email == model.Email && x.Id != id))
            {
                ModelState.AddModelError("", $"Email: '{model.Email}' already in use");
                return View(model);
            }
            if (await _dbContext.ContentCreator.AnyAsync(x => x.Email == model.Email))
            {
                ModelState.AddModelError("", $"Email: ' {model.Email} ' already in use");
                return View(model);
            }
            // also check in superadmin and admin table
            if (await _dbContext.SuperAdmin.AnyAsync(x => x.Email == model.Email))
            {
                ModelState.AddModelError("", $"Email: '{model.Email}' already in use");
                return View(model);
            }


            if (!string.IsNullOrEmpty(model.Password))
            {
                var hashedNewPassword = AuthHelper.HashPassword(model.Password);

                admin.Password = hashedNewPassword;
            }

            // Save the changes to the database
            await _dbContext.SaveChangesAsync();

            // Set a success message in TempData and redirect back to the index page
            TempData["SuccessMessage"] = "Editor updated successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Get the admin by id
            var admin = await _dbContext.Editor.FindAsync(id);

            // Check if admin exists
            if (admin == null)
            {
                return NotFound();
            }

            // Remove the admin from the database
            _dbContext.Editor.Remove(admin);
            await _dbContext.SaveChangesAsync();

            // Redirect to the index page with success message
            ViewBag.SuccessMessage = "Editor deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        #region Ajax

        [HttpPost("AjaxEditors")]
        public JsonResult GetContentCreators([FromBody] ContentCreatorListRequest request)
        {
            // get all content creators added by logged in user
            var query = _dbContext.Editor.Include(x => x.CreatedByUser).AsQueryable();

            int totalRecords = query.Count();

            query = query.Skip(request.PageNumber * request.PageSize).Take(request.PageSize);

            // we donot want to return whole data to the page
            var convertedObject = _mapper.Map<List<AdminListViewModel>>(query.ToList());

            return Json(new PaginatedAjaxResponse<AdminListViewModel>
            {
                Data = convertedObject,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalRecords = totalRecords
            });
        }
        #endregion
    }
}
