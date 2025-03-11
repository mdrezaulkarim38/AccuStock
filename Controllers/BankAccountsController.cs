using AccuStock.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AccuStock.Controllers;

public class BankAccountsController : Controller
{
    private readonly IBankAccountService _bankAccounts;
    private readonly IBranchService _branch;

    public BankAccountsController(IBankAccountService bankAccounts, IBranchService branch)
    {
        _bankAccounts = bankAccounts;
        _branch = branch;
    }

    [HttpGet]
    public async Task<IActionResult> BankAccountsList()
    {
        var branchList = await _branch.GetAllBranches();
        ViewBag.Branches = branchList;
        var bankAccountsList = await _bankAccounts.GetAllBankAccount();
        return View(bankAccountsList);
    }
}