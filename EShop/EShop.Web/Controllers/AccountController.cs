
using EShop.Domain.DomainModels;
using EShop.Domain.Idenitity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EShop.Web.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<EShopApplicationUser> userManager;
        private readonly SignInManager<EShopApplicationUser> signInManager;
        public AccountController(UserManager<EShopApplicationUser> userManager, SignInManager<EShopApplicationUser> signInManager)
        {

            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public IActionResult Register()
        {
            UserRegistrationDto model = new UserRegistrationDto();
            return View(model);
        }

        [HttpPost, AllowAnonymous]
        public IActionResult Register(UserRegistrationDto request)
        {
            if (ModelState.IsValid)
            {
                var userCheck = userManager.FindByEmailAsync(request.Email);
                userCheck.Wait();
                
                if (userCheck.Result == null)
                {
                    var user = new EShopApplicationUser
                    {
                        UserName = request.Email,
                        NormalizedUserName = request.Email,
                        Email = request.Email,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        UserCart = new ShoppingCart()
                    };
                    var resultTask =  userManager.CreateAsync(user, request.Password);
                    resultTask.Wait();

                    if (resultTask.Result.Succeeded)
                    {
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        if (resultTask.Result.Errors.Count() > 0)
                        {
                            foreach (var error in resultTask.Result.Errors)
                            {
                                ModelState.AddModelError("message", error.Description);
                            }
                        }
                        return View(request);
                    }
                }
                else
                {
                    ModelState.AddModelError("message", "Email already exists.");
                    return View(request);
                }
            }
            return View(request);

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            UserLoginDto model = new UserLoginDto();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(UserLoginDto model)
        {
            if (ModelState.IsValid)
            {
                var userTask = userManager.FindByEmailAsync(model.Email);
                userTask.Wait();
                if (userTask.Result != null && !userTask.Result.EmailConfirmed)
                {
                    ModelState.AddModelError("message", "Email not confirmed yet");
                    return View(model);

                }
                if ( userManager.CheckPasswordAsync(userTask.Result, model.Password).Result == false)
                {
                    ModelState.AddModelError("message", "Invalid credentials");
                    return View(model);

                }

                var result = signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true).Result;

                if (result.Succeeded)
                {
                    var resultOfCalimts = userManager.AddClaimAsync(userTask.Result, new Claim("UserRole", "Admin"));
                    resultOfCalimts.Wait();
                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    return View("AccountLocked");
                }
                else
                {
                    ModelState.AddModelError("message", "Invalid login attempt");
                    return View(model);
                }
            }
            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
