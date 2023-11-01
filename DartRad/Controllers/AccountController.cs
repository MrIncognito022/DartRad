using DartRad.Data;
using DartRad.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DartRad.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly AppDbContext _dbContext;

        public AccountController(AppDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult Profile()
        {
            var userId = this.GetUserId();
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            BaseUser userFromDb = null;
            if (role == AppRoles.SuperAdmin)
            {
                userFromDb = _dbContext.SuperAdmin.FirstOrDefault(x => x.Id == userId);
            }
            else if (role == AppRoles.Editor)
            {
                userFromDb = _dbContext.Editor.FirstOrDefault(x => x.Id == userId);
            }
            else
            {
                userFromDb = _dbContext.ContentCreator.FirstOrDefault(x => x.Id == userId);
            }

            var model = new ProfileViewModel
            {
                Name = userFromDb.Name,
                Email = userFromDb.Email
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // get logged in user id
            var userId = this.GetUserId();
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            BaseUser userFromDb = null;
            if (role == AppRoles.SuperAdmin)
            {
                userFromDb = _dbContext.SuperAdmin.FirstOrDefault(x => x.Id == userId);
                if (await _dbContext.SuperAdmin.AnyAsync(x => x.Email == model.Email && x.Id != this.GetUserId()))
                {
                    ModelState.AddModelError("", $"Email: ' {model.Email} ' already in use");
                    return View(model);
                }
                // also check in superadmin and admin table
                if (await _dbContext.Editor.AnyAsync(x => x.Email == model.Email))
                {
                    ModelState.AddModelError("", $"Email: '{model.Email}' already in use");
                    return View(model);
                }
                if (await _dbContext.ContentCreator.AnyAsync(x => x.Email == model.Email))
                {
                    ModelState.AddModelError("", $"Email: ' {model.Email} ' already in use");
                    return View(model);
                }
               
            }
            else if (role == AppRoles.Editor)
            {
                userFromDb = _dbContext.Editor.FirstOrDefault(x => x.Id == userId);
                if (await _dbContext.Editor.AnyAsync(x => x.Email == model.Email && x.Id != this.GetUserId()))
                {
                    ModelState.AddModelError("", $"Email: ' {model.Email} ' already in use");
                    return View(model);
                }
                if (await _dbContext.SuperAdmin.AnyAsync(x => x.Email == model.Email))
                {
                    ModelState.AddModelError("", $"Email: '{model.Email}' already in use");
                    return View(model);
                }
                if (await _dbContext.ContentCreator.AnyAsync(x => x.Email == model.Email))
                {
                    ModelState.AddModelError("", $"Email: ' {model.Email} ' already in use");
                    return View(model);
                }
            }
            else
            {
                userFromDb = _dbContext.ContentCreator.FirstOrDefault(x => x.Id == userId);
                if (await _dbContext.ContentCreator.AnyAsync(x => x.Email == model.Email && x.Id != this.GetUserId()))
                {
                    ModelState.AddModelError("", $"Email: ' {model.Email} ' already in use");
                    return View(model);
                }
                if (await _dbContext.Editor.AnyAsync(x => x.Email == model.Email))
                {
                    ModelState.AddModelError("", $"Email: '{model.Email}' already in use");
                    return View(model);
                }
                if (await _dbContext.SuperAdmin.AnyAsync(x => x.Email == model.Email))
                {
                    ModelState.AddModelError("", $"Email: ' {model.Email} ' already in use");
                    return View(model);
                }
            }


            // update user properties
            userFromDb.Name = model.Name;
            userFromDb.Email = model.Email;

          
            if (!string.IsNullOrEmpty(model.CurrentPassword))
            {
                // Validate new password if provided
                bool isPasswordCorrect = AuthHelper.VerifyPassword(model.CurrentPassword, userFromDb.Password);

                if (!isPasswordCorrect)
                {
                    ModelState.AddModelError("", "Current Password is incorrect");
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.NewPassword))
                {
                    ModelState.AddModelError("", "Please Provide a new password");
                    return View(model);
                }
                // Update password
                userFromDb.Password = AuthHelper.HashPassword(model.NewPassword);
            }

            // update user in db

            if (role == AppRoles.SuperAdmin)
            {
                _dbContext.SuperAdmin.Update(userFromDb as SuperAdmin);
            }
            else if (role == AppRoles.Editor)
            {
                _dbContext.Editor.Update(userFromDb as Editor);
            }
            else
            {
                _dbContext.ContentCreator.Update(userFromDb as ContentCreator);
            }

            await _dbContext.SaveChangesAsync();
            ViewBag.SuccessMessage = "Profile Updated Successfully";
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IActionResult> Register(string email, string inviteCode)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(inviteCode))
            {
                return RedirectToAction("Login", "Auth", new
                {
                    Area = ""
                });
            }

            // Verify invitation
            var invite = await _dbContext.ContentCreatorInvite
                .FirstOrDefaultAsync(i => i.Email.ToLower() == email.ToLower() && i.InvitationCode == inviteCode);

            if (invite == null)
            {
                TempData[TempDataKeys.SuccessMessage] = "Invalid Invite Link";
                return RedirectToAction("Login", "Auth", new
                {
                    Area = ""
                });

            }

            // check for existing registration
            var ccFromDb = await _dbContext.ContentCreator.FirstOrDefaultAsync(x => x.Email == email);

            // check if cc exists
            if (ccFromDb != null)
            {
                TempData[TempDataKeys.SuccessMessage] = "You are already a member, Please login";
                return RedirectToAction("Login", "Auth", new
                {
                    Area = ""
                });
            }

            var model = new ContentCreatorRegisterViewModel
            {
                Email = email,
                InvitationCode = inviteCode
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IActionResult> Register(ContentCreatorRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // check if invite exists and is valid
                var invite = await _dbContext.ContentCreatorInvite
                    .FirstOrDefaultAsync(i => i.Email == model.Email && i.InvitationCode == model.InvitationCode);

                if (invite == null)
                {
                    TempData[TempDataKeys.SuccessMessage] = "Invalid invitation code";
                    return RedirectToAction("Index", "Home", new
                    {
                        Area = ""
                    });
                }

                // check if content creator already exists
                if (await _dbContext.ContentCreator.AnyAsync(cc => cc.Email == model.Email))
                {
                    ModelState.AddModelError(string.Empty, "This email address is already registered as a content creator");
                    return View(model);
                }

                // create new content creator
                var contentCreator = new Entities.ContentCreator
                {
                    Email = model.Email,
                    Name = model.Name,
                    Password = AuthHelper.HashPassword(model.Password),
                    InvitedBy = invite.InvitedByAdminId,
                    CreatedAt = DateTime.Now
                };
                _dbContext.ContentCreator.Add(contentCreator);

                await _dbContext.SaveChangesAsync();
                TempData[TempDataKeys.SuccessMessage] = "Registration Successful, Please login";
                return RedirectToAction("Login", "Auth", new
                {
                    Area = ""
                });
            }

            return View(model);
        }
    }
}
