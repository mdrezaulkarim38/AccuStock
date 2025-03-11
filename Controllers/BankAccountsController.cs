using AccuStock.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AccuStock.Controllers;

public class BankAccountsController : Controller
{
    private readonly IBankAccountService _bankAccounts;

    public BankAccountsController(IBankAccountService bankAccounts)
    {
        _bankAccounts = bankAccounts;
    }

    [HttpGet]
    public async Task<IActionResult> BankAccountsList()
    {
        var bankAccountsList = await _bankAccounts.GetAllBankAccount();
        return View(bankAccountsList);
    }
}