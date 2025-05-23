using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccuStock.Controllers;

[Authorize]
public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly IBranchService _branchService;
    public UserController(ILogger<UserController> logger, IUserService userService, IBranchService branchService)
    {
        _logger = logger;
        _userService = userService;
        _branchService = branchService;
    }
    [HttpGet]
    public async Task<IActionResult> UserList()
    {
        var branchList = await _branchService.GetAllBranches();
        ViewBag.Branches = branchList;
        var userList = await _userService.GetAllUsers();
        return View(userList);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrUpdateUser(User user)
    {
        if (user.Id == 0)
        {
            bool isCreated = await _userService.CreateUser(user);
            if (!isCreated)
            {
                TempData["ErrorMessage"] = "A User already exists for this SubscriptionId.";
                return RedirectToAction("UserList");
            }
            TempData["SuccessMessage"] = "User Created Successfully";
        }
        else
        {
            bool isUpdated = await _userService.UpdateUser(user);
            if (!isUpdated)
            {
                TempData["ErrorMessage"] = "User name already exists or update failed";
                return RedirectToAction("UserList");
            }
            TempData["SuccessMessage"] = "User Updated Successfully";
        }
        return RedirectToAction("UserList");
    }

    [HttpPost]
    public async Task<IActionResult> ToggleUserStatus(int userId)
    {
        bool success = await _userService.ToggleUserStatusAsync(userId);

        if (!success)
            return NotFound();

        return RedirectToAction("UserList");
    }
}