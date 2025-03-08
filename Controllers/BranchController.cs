using AccuStock.Interface;
using Microsoft.AspNetCore.Mvc;
using AccuStock.Models;
using Microsoft.AspNetCore.Authorization;

namespace AccuStock.Controllers
{
    [Authorize]
    public class BranchController : Controller
    {
        private readonly IBranchService _BranchService;
        private readonly ILogger<BranchController> _logger;

        public BranchController(ILogger<BranchController> logger,IBranchService BranchService)
        {
            _logger = logger;
            _BranchService = BranchService;
        }

        [HttpGet]
        public async Task<IActionResult> Branch()
        {
            var branches = await _BranchService.GetAllBranches();
            return View(branches);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateBranch(Branch branch)
        {
            if (branch.Id == 0)
            {
                bool isCreated = await _BranchService.CreateBranch(branch);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "A Branch already exists for this SubscriptionId.";
                    return RedirectToAction("Branch");
                }
                TempData["SuccessMessage"] = "Branch Created Successfully";
            }
            else
            {
                bool isUpdated = await _BranchService.UpdateBranch(branch);
                if (!isUpdated)
                {
                    TempData["ErrorMessage"] = "Branch name already exists or update failed";
                    return RedirectToAction("Branch");
                }
                TempData["SuccessMessage"] = "Branch Updated Successfully";
            }

            return RedirectToAction("Branch");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var result = await _BranchService.DeleteBranch(id);

            if (result == "Branch not found.")
            {
                TempData["ErrorMessage"] = "Branch not found.";
                return RedirectToAction("Branch");
            }

            if (result == "Cannot delete this branch because users are assigned to it.")
            {
                TempData["ErrorMessage"] = "Cannot delete this branch because users are assigned to it.";
                return RedirectToAction("Branch");
            }

            TempData["SuccessMessage"] = "Branch deleted successfully.";
            return RedirectToAction("Branch");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}