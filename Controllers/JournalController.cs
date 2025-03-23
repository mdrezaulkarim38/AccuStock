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
    public async Task<IActionResult> CreateOrUpdateJournal([FromBody] JournalPost journal)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Invalid data" });
        }

        try
        {
            bool result;
            if (journal.Id == 0)
            {
                result = await _journalService.CreateJournal(journal);
            }
            else
            {
                result = await _journalService.UpdateJournal(journal);
            }

            return Json(new { success = result, message = result ? "Journal saved successfully" : "Failed to save journal" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}