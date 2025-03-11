using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AccuStock.Models.ViewModels.Auth;
using AccuStock.DTOS.AuthDto;
using AccuStock.Interface;

namespace AccuStock.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
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
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(loginViewModel);
        }
        try
        {
            var user = await _authService.LoginAsync(loginViewModel.Email!, loginViewModel.Password!);


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
            return View(loginViewModel);
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