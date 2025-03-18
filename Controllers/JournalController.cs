using AccuStock.Interface;
using AccuStock.Models;
using AccuStock.Services;
using Microsoft.AspNetCore.Mvc;

namespace AccuStock.Controllers
{
    public class JournalController : Controller
    {
        private readonly IjournalService _journalService;
        private readonly IBranchService _branch;

        public JournalController(IjournalService journalService, IBranchService branch)
        {
            _journalService = journalService;
            _branch = branch;
        }
        [HttpGet]
        public async Task<IActionResult> JournalList()
        {
            var JournalList = await _journalService.GetJournal();           
            return View(JournalList);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdatejournal(JournalPost journal)
        {
            //if (user.Id == 0)
            //{
            //    bool isCreated = await _userService.CreateUser(user);
            //    if (!isCreated)
            //    {
            //        TempData["ErrorMessage"] = "A User already exists for this SubscriptionId.";
            //        return RedirectToAction("UserList");
            //    }
            //    TempData["SuccessMessage"] = "User Created Successfully";
            //}
            //else
            //{
            //    bool isUpdated = await _userService.UpdateUser(user);
            //    if (!isUpdated)
            //    {
            //        TempData["ErrorMessage"] = "User name already exists or update failed";
            //        return RedirectToAction("UserList");
            //    }
            //    TempData["SuccessMessage"] = "User Updated Successfully";
            //}
            return RedirectToAction("JournalList");
        }
    }
}
