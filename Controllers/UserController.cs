using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AccuStock.Interface;
using AccuStock.Models;
using AccuStock.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AccuStock.Controllers
{
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
                    return RedirectToAction("Branch");
                }
                TempData["SuccessMessage"] = "User Created Successfully";
            }
            else
            {
                bool isUpdated = await _userService.UpdateUser(user);
                if (!isUpdated)
                {
                    TempData["ErrorMessage"] = "User name already exists or update failed";
                    return RedirectToAction("Branch");
                }
                TempData["SuccessMessage"] = "User Updated Successfully";
            }

            return RedirectToAction("UserList");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}