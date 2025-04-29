using AccuStock.Interface;
using AccuStock.Models;
using AccuStock.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AccuStock.Controllers
{
    public class SaleController : Controller
    {
        private readonly ISaleService _saleService;
        public SaleController(ISaleService saleService)
        {
            _saleService = saleService;
        }
        
        public async Task<IActionResult> Sales()
        {
            var sales = await _saleService.GetAllSale();
            return View(sales);
        }

        [HttpGet]
        public async Task<IActionResult> AddOrEditSale(int id = 0)
        {
            if (id == 0)
            {
                return View(new Sale());
            }
            else
            {
                var sale = await _saleService.GetSalebyId(id);
                if (sale == null)
                {
                    TempData["ErrorMessage"] = "Sale not found!";
                    return RedirectToAction("Sale");
                }
                return View(sale);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddOrEditSale(Sale sale)
        {
            if (sale.Id == 0)
            {
                bool isCreated = await _saleService.CreateSale(sale);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "A Sale already exists for this SubscriptionId.";
                    return RedirectToAction("Sale");
                }
                TempData["SuccessMessage"] = "Sale Created Successfully";
            }
            else
            {
                bool isUpdated = await _saleService.UpdateSale(sale);
                if (!isUpdated)
                {
                    TempData["ErrorMessage"] = "Sale already exists or update failed";
                    return RedirectToAction("Sale");
                }
                TempData["SuccessMessage"] = "Sale Updated Successfully";
            }
            return RedirectToAction("Sale");
        }
    }
}
