using AccuStock.Interface;
using AccuStock.Models;
using AccuStock.Models.ViewModels.Sale;
using AccuStock.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AccuStock.Controllers
{
    public class SaleController : Controller
    {
        private readonly ISaleService _saleService;
        private readonly ICustomerService _customerService;
        public SaleController(ISaleService saleService, ICustomerService customerService )
        {
            _saleService = saleService;
            _customerService = customerService;
        }
        
        public async Task<IActionResult> Sales()
        {
            var sales = await _saleService.GetAllSale();
            return View(sales);
        }
        [HttpGet]
        public async Task<IActionResult> AddOrEditSale(int id = 0)
        {
            //var model = new SaleViewModel
            //{
            //    SaleDate = DateTime.Now,
            //    CustomerList = new SelectList(await _customerService.GetAllCustomer(), "Id", "Name").ToList(),
            //    Details = new List<SaleDetailViewModel>() // empty initially
            //};

            //if (id != 0)
            //{
            //    var sale = await _saleService.GetSalebyId(id);
            //    if (sale == null)
            //    {
            //        TempData["ErrorMessage"] = "Sale not found!";
            //        return RedirectToAction("Sale");
            //    }

            //    // Populate the ViewModel with existing sale data
            //    model.Id = sale.Id;
            //    model.CustomerId = sale.CustomerId;
            //    model.BranchId = sale.BranchId;
            //    model.SaleDate = sale.SaleDate;
            //    model.Notes = sale.Notes;
            //    model.PaymentMethod = sale.PaymentMethod;

            //    // Map sale details
            //    model.Details = sale.SaleDetails.Select(d => new SaleDetailViewModel
            //    {
            //        ProductId = d.ProductId,
            //        Quantity = d.Quantity,
            //        UnitPrice = d.UnitPrice,
            //        VatRate = d.VatRate
            //    }).ToList();
            //}
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddOrEditSale(Sale sale)
        {
            ViewBag.Customers = new SelectList(await _customerService.GetAllCustomer(), "Id", "Name");

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
