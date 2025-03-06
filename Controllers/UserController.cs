using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AccuStock.Interface;
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
            return View();
        }

        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}