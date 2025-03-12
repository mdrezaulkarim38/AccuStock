using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccuStock.Controllers;

[Authorize]
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

    [HttpPost]
    public async Task<IActionResult> ToggleBankStatus(int bankId)
    {
        bool success = await _bankAccounts.ToggleBankStatus(bankId);

        if (!success)
            return NotFound();

        return RedirectToAction("BankAccountsList");
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrUpdateBank(BankAccount bankAccount)
    {
        if (bankAccount.Id == 0)
        {
            bool isCreated = await _bankAccounts.CreateBank(bankAccount);
            if (!isCreated)
            {
                TempData["ErrorMessage"] = "A User already exists for this SubscriptionId.";
                return RedirectToAction("BankAccountsList");
            }
            TempData["SuccessMessage"] = "User Created Successfully";
        }
        else
        {
            bool isUpdated = await _bankAccounts.UpdateBank(bankAccount);
            if (!isUpdated)
            {
                TempData["ErrorMessage"] = "User name already exists or update failed";
                return RedirectToAction("BankAccountsList");
            }
            TempData["SuccessMessage"] = "User Updated Successfully";
        }
        return RedirectToAction("BankAccountsList");
    }
}