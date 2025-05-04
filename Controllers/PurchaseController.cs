using AccuStock.Interface;
using AccuStock.Models;
using AccuStock.Models.ViewModels.Purchase;
using AccuStock.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AccuStock.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly IPurchaseService _purchaseService;
        private readonly IVendor _vendorService;
        private readonly IProductService _productService;
        private readonly IBranchService _branchService;
        public PurchaseController(IPurchaseService purchaseService, IVendor vendorService, IProductService productService, IBranchService branchService)
        {
            _purchaseService = purchaseService;
            _vendorService = vendorService;
            _productService = productService;
            _branchService = branchService;
        }
        public async Task<IActionResult> Purchase()
        {
            var purchase = await _purchaseService.GetAllPurchase();
            return View(purchase);
        }
        [HttpGet]
        public async Task<IActionResult> AddOrEditPurchase(int id = 0)
        {
            var vendors = await _vendorService.GetAllVendor();
            var products = await _productService.GetAllProduct();
            var branches = await _branchService.GetAllBranches();

            var viewModel = new PurchaseViewModel
            {
                VendorList = new SelectList(vendors, "Id", "Name"),
                BranchList = new SelectList(branches, "Id", "Name"),
                PurchaseDate = DateTime.Now,
                Details = new List<PurchaseDetailViewModel> { new PurchaseDetailViewModel() }
            };

            ViewBag.ProductList = new SelectList(products, "Id", "Name");
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> AddOrEditPurchase(PurchaseViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var vendors = await _vendorService.GetAllVendor();
                var products = await _productService.GetAllProduct();
                var branches = await _branchService.GetAllBranches();
                viewModel.VendorList = new SelectList(vendors, "Id", "Name");
                viewModel.BranchList = new SelectList(branches, "Id", "Name");
                ViewBag.ProductList = new SelectList(products, "Id", "Name");
                return View(viewModel);
            }

            var purchase = new Purchase
            {
                Id = viewModel.Id,
                VendorId = viewModel.VendorId,
                BranchId = viewModel.BranchId,
                PurchaseDate = viewModel.PurchaseDate,
                Notes = viewModel.Notes,
                PurchaseStatus = 0,
                Details = viewModel.Details.Select(d => new PurchaseDetail
                {
                    ProductId = d.ProductId,
                    Quantity = (int)d.Quantity,
                    UnitPrice = d.UnitPrice,
                    VatRate = d.VatRate
                }).ToList()
            };

            try
            {
                bool isCreated = await _purchaseService.CreatePurchase(purchase);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "Failed to create purchase.";
                    return RedirectToAction("Purchase");
                }
                TempData["SuccessMessage"] = "Purchase Created Successfully";
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                var vendors = await _vendorService.GetAllVendor();
                var products = await _productService.GetAllProduct();
                var branches = await _branchService.GetAllBranches();
                viewModel.VendorList = new SelectList(vendors, "Id", "Name");
                viewModel.BranchList = new SelectList(branches, "Id", "Name");
                ViewBag.ProductList = new SelectList(products, "Id", "Name");
                return View(viewModel);
            }
            return RedirectToAction("Purchase");
        }
    }
}
