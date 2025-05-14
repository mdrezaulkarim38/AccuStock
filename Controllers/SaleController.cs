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
        private readonly IBranchService _branchService;
        private readonly IProductService _productService;
        public SaleController(ISaleService saleService, ICustomerService customerService, IBranchService branchService, IProductService productService )
        {
            _saleService = saleService;
            _customerService = customerService;
            _branchService = branchService;
            _productService = productService;
        }
        
        public async Task<IActionResult> Sales()
        {
            var sales = await _saleService.GetAllSale();
            return View(sales);
        }
        [HttpGet]
        public async Task<IActionResult> AddOrEditSale(int id = 0)
        {
            var model = new SaleViewModel
            {
                SaleDate = DateTime.Now,
                CustomerList = new SelectList(await _customerService.GetAllCustomer(), "Id", "Name").ToList(),
                BranchList = new SelectList(await _branchService.GetAllBranches(), "Id", "Name").ToList(),
                ProductList = new SelectList(await _productService.GetAllProduct(), "Id", "Name").ToList(),
                Details = new List<SaleDetailVM>()
            };

            if (id != 0)
            {
                var sale = await _saleService.GetSalebyId(id);
                if (sale == null)
                {
                    TempData["ErrorMessage"] = "Sale not found!";
                    return RedirectToAction("Sales");
                }

                model.Id = sale.Id;
                model.CustomerId = sale.CustomerId;
                model.BranchId = sale.BranchId;
                model.SaleDate = sale.InvoiceDate;
                model.PaymentMethod = sale.PaymentMethod?.ToString();
                //model.Notes = sale.Notes;

                model.Details = sale.SaleDetails?.Select(d => new SaleDetailVM
                {
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    VatRate = d.VatRate
                }).ToList() ?? new List<SaleDetailVM>();
            }
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddOrEditSale(SaleViewModel viewModel)
        {
            ViewBag.Customers = new SelectList(await _customerService.GetAllCustomer(), "Id", "Name");
            var sale = new Sale
            {
                Id = viewModel.Id,
                InvoiceDate = viewModel.SaleDate,
                CustomerId = viewModel.CustomerId,
                BranchId = viewModel.BranchId,
                PaymentMethod = Convert.ToInt32(viewModel.PaymentMethod),
                Notes = viewModel.Notes!,
                SaleDetails = viewModel.Details.Select(d => new SaleDetails
                {
                    ProductId = d.ProductId,
                    Quantity = (int)d.Quantity,
                    UnitPrice = d.UnitPrice,
                    VatRate = d.VatRate
                }).ToList()
            };
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
            return RedirectToAction("Sales");
        }
    }
}
