using AutoMapper;
using DartRad.Areas.Editor.Models;
using DartRad.Data;
using DartRad.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;


namespace DartRad.Areas.Editor.Controllers
{
    [Area("Editor")]
    [Authorize(Roles = AppRoles.Editor)]
    [Route("[Area]/[Controller]")]
    public class ContentCreatorsController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;

        public ContentCreatorsController(AppDbContext dbContext, IMapper mapper, IMailService mailService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            this._mailService = mailService;
        }

        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("Invite")]
        public IActionResult Invite()
        {
            return View();
        }

        [HttpPost]
        [Route("Invite")]
        public async Task<IActionResult> Invite(ContentCreatorInviteViewModel model)
        {
            if (ModelState.IsValid)
            {
                // check in db
                var ccFromDb = _dbContext.ContentCreator.FirstOrDefault(x => x.Email == model.Email);
                if (ccFromDb != null)
                {
                    ModelState.AddModelError("", "Content Creator already joined");
                    return View(model);
                }

                // Generate a unique invitation code
                string invitationCode = InviteCodeGenerator.GenerateInviteCode();

                // Get the ID of the currently logged-in cc
                int invitedByccId = GetUserId();

                // before creating a new invite check for previous, if he's been invited before, update the invite code

                var inviteFromDb = _dbContext.ContentCreatorInvite.FirstOrDefault(x => x.Email == model.Email);

                if (inviteFromDb != null)
                {
                    inviteFromDb.InvitationCode = invitationCode;
                    // update
                    _dbContext.ContentCreatorInvite.Update(inviteFromDb);
                }
                else
                {

                    var invite = new ContentCreatorInvite
                    {
                        Email = model.Email,
                        InvitationCode = invitationCode,
                        InvitedByAdminId = invitedByccId
                    };

                    // Add the invite to the database
                    _dbContext.ContentCreatorInvite.Add(invite);
                }

                await _dbContext.SaveChangesAsync();

                // Construct the URL for the registration page
                string registrationUrl = Url.Action("Register", "Account", new
                {
                    email = model.Email,
                    inviteCode = invitationCode
                }, Request.Scheme);

                // Send an email to the invited user with a link to complete their registration
                string emailBody = $"You have been invited to register as a content creator for our site. Click the following link to complete your registration: {registrationUrl}";
                _mailService.Send(new EMailMessage
                {
                    Body = emailBody,
                    ReceiverEmail = model.Email,
                    Subject = "Invitation to register as a content creator"
                });


                TempData[TempDataKeys.SuccessMessage] = "The content creator has been invited successfully";

               
                //TempData[TempDataKeys.AcceptInviteLink] = registrationUrl;
                return RedirectToAction("Index");
            }

            return View(model);


        }

        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var ccFromDb = await _dbContext.ContentCreator.FindAsync(id);

            // check if cc exists
            if (ccFromDb == null)
            {
                return NotFound();
            }

            var ccViewModel = _mapper.Map<ContentCreatorUpdateViewModel>(ccFromDb);
            return View(ccViewModel);
        }

        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> Edit(int id, ContentCreatorUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get the cc from the database
            var cc = await _dbContext.ContentCreator.FindAsync(id);

            if (cc == null)
            {
                return NotFound();
            }

            // Update the cc with the new data
            cc.Name = model.Name;
            cc.Email = model.Email;

            // validate for duplicate email
            if (await _dbContext.ContentCreator.AnyAsync(x => x.Email == model.Email && x.Id != id))
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
            if (await _dbContext.Editor.AnyAsync(x => x.Email == model.Email))
            {
                ModelState.AddModelError("", $"Email: '{model.Email}' already in use");
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.Password))
            {
                var hashedNewPassword = AuthHelper.HashPassword(model.Password);

                cc.Password = hashedNewPassword;
            }

            // Save the changes to the database
            await _dbContext.SaveChangesAsync();

            // Set a success message in TempData and redirect back to the index page
            TempData[TempDataKeys.SuccessMessage] = "Content Creator updated successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            // Get the cc by id
            var cc = await _dbContext.ContentCreator.FindAsync(id);

            // Check if cc exists
            if (cc == null)
            {
                return NotFound();
            }

            // Remove the cc from the database
            _dbContext.ContentCreator.Remove(cc);
            await _dbContext.SaveChangesAsync();

            // Redirect to the index page with success message
            TempData[TempDataKeys.SuccessMessage] = "Content Creator deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        #region Ajax

        [HttpPost("AjaxContentCreators")]
        public JsonResult GetContentCreators([FromBody] ContentCreatorListRequest request)
        {
            // get all content creators added by logged in user
            var query = _dbContext.ContentCreator.Include(x => x.InvitedByUser).AsQueryable();

            int totalRecords = query.Count();

            query = query.Skip(request.PageNumber * request.PageSize).Take(request.PageSize);

            // convert the object to viewmodel
            // we donot want to return whole data to the page
            var convertedObject = _mapper.Map<List<ContentCreatorListViewModel>>(query.ToList());

            return Json(new PaginatedAjaxResponse<ContentCreatorListViewModel>
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
