using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccuStock.Controllers;

[Authorize]
public class JournalController : Controller
{
    private readonly IJournalService _journalService;
    private readonly IBranchService _branch;

    public JournalController(IJournalService journalService, IBranchService branch)
    {
        _journalService = journalService;
        _branch = branch;
    }

    [HttpGet]
    public async Task<IActionResult> JournalList()
    {
        var journalList = await _journalService.GetJournal();
        return View(journalList);
    }

    [HttpGet]
    public IActionResult AddJournal()
    {
        return View();
    }

    [HttpPost]
    public Task<IActionResult> CreateOrUpdatejournal(JournalPost journal)
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
        return Task.FromResult<IActionResult>(RedirectToAction("JournalList"));
    }
}