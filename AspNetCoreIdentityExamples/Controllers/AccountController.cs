using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreIdentityExamples.Models;
using AspNetCoreIdentityExamples.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using EmailService;

namespace AspNetCoreIdentityExamples.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AppUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = new AppUser
                {
                    UserName = model.UserName,
                    Email = model.Email
                };
                IdentityResult result = await _userManager.CreateAsync(appUser, model.NotHashedPassword);
                if (result.Succeeded)
                {
                    string token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
                    await SendConfirmationLinkAsync(token,appUser.Email);
                    return RedirectToAction("ConfirmationMailHasSent");
                }
                else
                    result.Errors.ToList().ForEach(e => ModelState.AddModelError("", e.Description));
            }


            return View(model);
        }

        public async Task<IActionResult> ResendConfirmationLink(string email) {


            AppUser user = await _userManager.FindByEmailAsync(email);

            //Əvvəlki email confirmation linkin artıq keçərli olmamasını təmin edir
            await _userManager.UpdateSecurityStampAsync(user);
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            await SendConfirmationLinkAsync(token,email);
            return RedirectToAction("ConfirmationMailHasSent");
        }


        [NonAction]
        private async Task SendConfirmationLinkAsync(string token, string email)
        {
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { token, email }, Request.Scheme);
            var message = new Message(new[] { email }, "Confirm your email", link);
            await _emailSender.SendEmailAsync(message);
            
        }



        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            AppUser user = await _userManager.FindByEmailAsync(email);
            if (user == null) {
                return View("Error");   
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return View(result.Succeeded ? nameof(ConfirmEmail) : "Error");
        }

        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ConfirmationMailHasSent()
        {
            return View();
        }

        public IActionResult Login(string ReturnUrl)
        {
            TempData["returnUrl"] = ReturnUrl;
            return View();
        }




        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Əgər validasiya problemi yoxdursa, get useri axtar
            //User yoxdursa, kənar şəxslər bu emaillə hesab açılıb açılmadığı kontrol edə bilməsin deyə bu uyarını verək
            AppUser user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("UserIsNotFound", "This email or username does not exist.");
                return View(model);
            }

            //Problem yoxdursa, yola davam. Mövcud cookieləri təmizləyək, və sign-in cəhdi edək
            await _signInManager.SignOutAsync();

            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, model.Password, model.Persistent, true);
            // Hər hansı səbəbdən sign-in uğursuz oldu deyək ki
            if (!result.Succeeded)
            {

                //Uğursuzluğun səbəblərini araşdıraq

                ViewBag.DoesNeedForConfirmation = false;
                //Email təsdiqlənməyib
                var isConfirmed = await _userManager.IsEmailConfirmedAsync(user);
                if (!isConfirmed)
                {
                    ModelState.AddModelError("NotConfirmed", "You must verify your account to log in. A confirmation link has been sent to your email inbox.");
                    ViewBag.DoesNeedForConfirmation = true;

                }
                // 2.Hesab kilidlidir
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Locked", "Your account has been locked because you have failed to login 3 times in a row. Please try again in 5 minutes.");
                }
                //Şifrə səhvdir.
                else {
                    ModelState.AddModelError("WrongCredentials", "The e-mail or password is incorrect.");
                }
                return View(model);


            }


            // Yox əgər uğurlu oldusa burdan davam etsin
            if (string.IsNullOrEmpty(TempData["returnUrl"] != null ? TempData["returnUrl"].ToString() : ""))
            {
                return RedirectToAction("Index");
            }

            return Redirect(TempData["returnUrl"].ToString());


        }




        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }


        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
                if (user == null)
                {
                    return RedirectToAction("ForgotPasswordConfirmation");
                }
                await _userManager.UpdateSecurityStampAsync(user);
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await SendResetPasswordLink(user.Email, token);
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }




            return View(forgotPasswordModel);
        }

        [NonAction]
        private async Task SendResetPasswordLink(string email, string token)
        {
            var callback = Url.Action(nameof(ResetPassword), "Account", new { token, email}, Request.Scheme);
            var message = new Message(new string[] { email }, "Reset password token", callback);
            await _emailSender.SendEmailAsync(message);
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordModel { Token = token, Email = email };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {

            if (!ModelState.IsValid)
            {
                return View(resetPasswordModel);
            }

            AppUser  user  = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
            if (user == null)
            {
                RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            IdentityResult result = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.Password);
            var errors = result.Errors.ToList();
            if (!result.Succeeded) {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }


            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }
        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
