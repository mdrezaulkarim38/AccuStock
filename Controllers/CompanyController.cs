using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.AspNetCore.Mvc;

namespace AccuStock.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ILogger<CompanyController> _logger;
        private readonly ICompanyService _companyService;

        public CompanyController(ILogger<CompanyController> logger, ICompanyService companyService)
        {
            _logger = logger;
            _companyService = companyService;
        }

        [HttpGet]
        public async Task<IActionResult> Company()
        {
            var company = await _companyService.GetCompanyBySubscriptionId();
            if (company == null)
            {
                return View(new Company());
            }
            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Company(Company company)
        {
            if (!ModelState.IsValid)
            {
                return View(company);
            }

            try
            {
                if (company.Id == 0)
                {
                    bool isCreated = await _companyService.CreateCompanyAsync(company);
                    if (!isCreated)
                    {
                        TempData["ErrorMessage"] = "A company already exists for this SubscriptionId.";
                        return View(company);
                    }
                }
                else
                {
                    bool isUpdated = await _companyService.UpdateCompanyAsync(company);
                    if (!isUpdated)
                    {
                        TempData["ErrorMessage"] = "Company name already exists or update failed";
                        return View(company);
                    }
                }
                TempData["SuccessMessage"] = "Company Created Successfully";
                return RedirectToAction("Company");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);
                return View(company);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}