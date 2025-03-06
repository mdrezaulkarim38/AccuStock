using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AccuStock.Interface;
using Microsoft.AspNetCore.Mvc;
using AccuStock.Models;
using Microsoft.Extensions.Logging;

namespace AccuStock.Controllers
{
    public class BranchController : Controller
    {
        private readonly ISettingService _SettingService;
        private readonly ILogger<BranchController> _logger;

        public BranchController(ILogger<BranchController> logger,ISettingService SettingService)
        {
            _logger = logger;
            _SettingService = SettingService;
        }

        [HttpGet]
        public async Task<IActionResult> Branch()
        {
            var branches = await _SettingService.GetAllBranches();
            return View(branches);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateBranch(Branch branch)
        {
            if (branch.Id == 0)
            {
                bool isCreated = await _SettingService.CreateBranch(branch);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "A Branch already exists for this SubscriptionId.";
                    return RedirectToAction("Branch");
                }
                TempData["SuccessMessage"] = "Branch Created Successfully";
            }
            else
            {
                bool isUpdated = await _SettingService.UpdateBranch(branch);
                if (!isUpdated)
                {
                    TempData["ErrorMessage"] = "Branch name already exists or update failed";
                    return RedirectToAction("Branch");
                }
                TempData["SuccessMessage"] = "Branch Updated Successfully";
            }

            return RedirectToAction("Branch");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}