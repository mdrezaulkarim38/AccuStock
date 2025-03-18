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
    }
}
