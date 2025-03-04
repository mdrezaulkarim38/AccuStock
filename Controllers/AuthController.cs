using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using AccuStock.Data;
using System.Security.Claims;
using AccuStock.DTOS.AuthDto;
using AccuStock.Models;
using AccuStock.Services;
using AccuStock.Interface;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AccuStock.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return View(registerDto);
            }

            try
            {
                var user = await _authService.RegisterAsync(registerDto);
                TempData["SuccessMessage"] = "Registration successful! Please log in.";
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {

                TempData["ErrorMessage"] = "Registration Unsuccessful!" + ex.Message;
                return View(registerDto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View(new LoginDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }
            try
            {
                var user = await _authService.LoginAsync(loginDto.Email!, loginDto.Password!);


                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.FullName!),
                    new Claim(ClaimTypes.Email, user.Email!),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.RoleId == 1 ? "SuperAdmin" : user.RoleId == 2 ? "Admin" : "Operator"),
                    new Claim("SubscriptionId", user.SubscriptionId.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimsPrincipal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                        AllowRefresh = true
                    });

                TempData["SuccessMessage"] = $"Welcome back, {user.FullName}!";
                return RedirectToAction("Dashboard", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Login Unsuccessful!" + ex.Message;
                return View(loginDto);
            }
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }

        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
    }
}
