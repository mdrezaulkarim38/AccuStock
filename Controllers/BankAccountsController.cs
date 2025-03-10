using AccuStock.Interface;
using AccuStock.Services;
using Microsoft.AspNetCore.Mvc;

namespace AccuStock.Controllers
{
    public class BankAccountsController : Controller
    {       
        private readonly IBankAccounts _bankAccounts;
        private readonly IBranchService _branchService;

        public BankAccountsController(IBankAccounts bankAccounts, IBranchService branchService)
        {
             _bankAccounts = bankAccounts;
            _branchService = branchService;
        }
        [HttpGet]
        public async Task<IActionResult> BankAccountsList()
        {
            var branchList = await _branchService.GetAllBranches();
            ViewBag.Branches = branchList;
            var BankAccountsList = await _bankAccounts.GetAllBannk();
            return View(BankAccountsList);
        }
    }
}
