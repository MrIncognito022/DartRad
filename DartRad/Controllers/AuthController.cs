using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DartRad.Utilities;
using Microsoft.AspNetCore.Identity;
using DartRad.Data;
using System.Data;

namespace DartRad.Controllers
{
    [Route("[Controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly AppDbContext _dbContext;

        public AuthController(IAuthService authService, AppDbContext dbContext)
        {
            this._authService = authService;
            this._dbContext = dbContext;
        }

        [AllowAnonymous]
        [HttpGet("ContentCreators")]

        public IActionResult Login(string returnUrl)
        {
            // Store the return URL in a TempData
            TempData["ReturnUrl"] = returnUrl;
            return View();
        }
        [AllowAnonymous]
        [HttpGet("Editor")]
        public IActionResult AdminLogin()
        {
            return View();
        }
        
        [AllowAnonymous]
        [HttpGet("SuperAdmin")]
        public IActionResult SuperAdminLogin()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost("ContentCreators")]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            model.Role = AppRoles.ContentCreator;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.Authenticate(model.Email, model.Password, model.Role);

            if (result.IsSuccess)
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Email, result.User.Email),
                    new Claim(ClaimTypes.Role, model.Role),
                    new Claim(ClaimTypes.Name, result.User.Name),
                    new Claim(ClaimTypes.NameIdentifier, result.User.Id.ToString())
                 };
                //Initialize a new instance of the ClaimsIdentity with the claims and authentication scheme    
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                //Initialize a new instance of the ClaimsPrincipal with ClaimsIdentity    
                var principal = new ClaimsPrincipal(identity);
                //SignInAsync is a Extension method for Sign in a principal for the specified scheme.    
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties()
                {
                    IsPersistent = true
                });


                string storedReturnUrl = TempData["ReturnUrl"]?.ToString();

                if (!string.IsNullOrEmpty(storedReturnUrl) && Url.IsLocalUrl(storedReturnUrl))
                {
                    // Redirect to the stored return URL
                    return LocalRedirect(storedReturnUrl);
                }
                else
                {
                    // If no valid return URL, redirect to a default route
                    return RedirectToAction("Index", "Home", new { Area = "ContentCreator" });

                }
            }
            else
            {
                ModelState.AddModelError("", result.Error);
                return View(model);
            }

        }

        [AllowAnonymous]
        [HttpPost("Editor")]
        public async Task<IActionResult> AdminLogin(LoginViewModel model)
        {
            model.Role = AppRoles.Editor;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.Authenticate(model.Email, model.Password, model.Role);

            if (result.IsSuccess)
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Email, result.User.Email),
                    new Claim(ClaimTypes.Role, model.Role),
                    new Claim(ClaimTypes.Name, result.User.Name),
                    new Claim(ClaimTypes.NameIdentifier, result.User.Id.ToString())
                 };
                //Initialize a new instance of the ClaimsIdentity with the claims and authentication scheme    
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                //Initialize a new instance of the ClaimsPrincipal with ClaimsIdentity    
                var principal = new ClaimsPrincipal(identity);
                //SignInAsync is a Extension method for Sign in a principal for the specified scheme.    
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties()
                {
                    IsPersistent = true
                });
                return RedirectToAction("Index", "Home", new
                {
                    Area = "Editor"
                });
            }
            else
            {
                ModelState.AddModelError("", result.Error);
                return View(model);
            }

        }

        [AllowAnonymous]
        [HttpPost("SuperAdmin")]
        public async Task<IActionResult> SuperAdminLogin(LoginViewModel model)
        {
            model.Role = AppRoles.SuperAdmin;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.Authenticate(model.Email, model.Password, model.Role);

            if (result.IsSuccess)
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Email, result.User.Email),
                    new Claim(ClaimTypes.Role, model.Role),
                    new Claim(ClaimTypes.Name, result.User.Name),
                    new Claim(ClaimTypes.NameIdentifier, result.User.Id.ToString())
                 };
                //Initialize a new instance of the ClaimsIdentity with the claims and authentication scheme    
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                //Initialize a new instance of the ClaimsPrincipal with ClaimsIdentity    
                var principal = new ClaimsPrincipal(identity);
                //SignInAsync is a Extension method for Sign in a principal for the specified scheme.    
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties()
                {
                    IsPersistent = true
                });
                return RedirectToAction("Index", "Home", new
                {
                    Area = "SuperAdmin"
                });
            }
            else
            {
                ModelState.AddModelError("", result.Error);
                return View(model);
            }

        }

        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Auth");
        }

        [HttpGet("AccessDenied")]
        public IActionResult AccessDenied(string returnUrl)
        {
            return View();
        }

        [HttpGet("ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // need to figure out where our user belongs to
            BaseUser user = _dbContext.SuperAdmin.FirstOrDefault(x => x.Email == model.Email);
            if(user == null)
            {
                // find in admins
                user = _dbContext.Editor.FirstOrDefault(x => x.Email == model.Email);
            }
            if(user == null)
            {
                user = _dbContext.ContentCreator.FirstOrDefault(x => x.Email == model.Email);
            }

            if (user == null)
            {
                // Don't reveal that the user does not exist or is not confirmed
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            var token = TokenHelper.GenerateResetPasswordToken();

            _dbContext.ResetPasswordRequest.Add(new ResetPasswordRequest
            {
                Email = model.Email,
                IsUsed = false,
                RequestTime = DateTime.UtcNow,
                Token = token
            });

            await _dbContext.SaveChangesAsync();

            // Send email with password reset link
            // ...
            string resetLink = "";
            TempData[TempDataKeys.ResetPasswordLink] = resetLink =  Url.Action("ResetPassword", "Auth", new
            {
                email = model.Email,
                token = token
            });

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }
        [HttpGet("ForgotPasswordConfirmation")]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet("ResetPassword")]
        public IActionResult ResetPassword(string email, string token)
        {
            if (email == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new ResetPasswordViewModel { Email = email, Token = token };
            return View(model);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // check the validity of the reset request
            var requestFromDb = _dbContext.ResetPasswordRequest.FirstOrDefault(x => x.Email == model.Email
                && x.Token == model.Token && x.IsUsed == false
            );

            if(requestFromDb == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // identify the user
            string userRole = AppRoles.SuperAdmin;
            BaseUser user = _dbContext.SuperAdmin.FirstOrDefault(x => x.Email == model.Email);
            if (user == null)
            {
                // find in admins
                user = _dbContext.Editor.FirstOrDefault(x => x.Email == model.Email);
                userRole = AppRoles.Editor;
            }
            if (user == null)
            {
                user = _dbContext.ContentCreator.FirstOrDefault(x => x.Email == model.Email);
                userRole = AppRoles.ContentCreator;
            }

            if (user == null)
            {
                return RedirectToAction(nameof(Login));
            }

            user.Password = AuthHelper.HashPassword(model.Password);

            if (userRole == AppRoles.SuperAdmin)
            {
                _dbContext.SuperAdmin.Update(user as SuperAdmin);
            }
            else if (userRole == AppRoles.Editor)
            {
                _dbContext.Editor.Update(user as Editor);
            }
            else
            {
                _dbContext.ContentCreator.Update(user as ContentCreator);
            }

            await _dbContext.SaveChangesAsync();

            requestFromDb.IsUsed = true;
            _dbContext.ResetPasswordRequest.Update(requestFromDb);
            await _dbContext.SaveChangesAsync();

            TempData[TempDataKeys.SuccessMessage] = "Password Changed Successfully";
            return RedirectToAction(nameof(Login));

        }
    }
}
