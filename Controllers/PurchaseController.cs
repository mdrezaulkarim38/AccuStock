﻿using AccuStock.Interface;
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

            //ViewBag.ProductList = new SelectList(products, "Id", "Name");

            if (id != 0)
            {
                var purchase = await _purchaseService.GetPurchasebyId(id);

                if (purchase != null)
                {
                    var purchaseViewModel = new PurchaseViewModel
                    {
                        Id = purchase.Id,
                        VendorId = purchase.VendorId,
                        BranchId = purchase.BranchId,
                        PaymentMethod = purchase.PaymentMethod,
                        PurchaseDate = purchase.PurchaseDate,
                        Notes = purchase.Notes,
                        PurchaseStatus = purchase.PurchaseStatus,
                        VendorList = new SelectList(vendors, "Id", "Name", purchase.VendorId),
                        BranchList = new SelectList(branches, "Id", "Name", purchase.BranchId),
                        ProductList = new SelectList(products, "Id", "Name", purchase.Id),
                        Details = purchase.Details != null
                ? purchase.Details.Select(d => new PurchaseDetailViewModel
                {
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    VatRate = d.VatRate
                }).ToList()
                : new List<PurchaseDetailViewModel> { new PurchaseDetailViewModel() } 
                    };

                    return View(purchaseViewModel);
                }

                // If purchase not found
                return View(new PurchaseViewModel
                {
                    VendorList = new SelectList(vendors, "Id", "Name"),
                    BranchList = new SelectList(branches, "Id", "Name"),
                    ProductList = new SelectList(products,"Id", "Name"),
                    PurchaseDate = DateTime.Now,
                    Details = new List<PurchaseDetailViewModel> { new PurchaseDetailViewModel() }
                });
            }

            // New purchase case
            return View(new PurchaseViewModel
            {
                VendorList = new SelectList(vendors, "Id", "Name"),
                BranchList = new SelectList(branches, "Id", "Name"),
                ProductList = new SelectList(products,"Id", "Name"),
                PurchaseDate = DateTime.Now,
                Details = new List<PurchaseDetailViewModel> { new PurchaseDetailViewModel() }
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddOrEditPurchase(PurchaseViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(viewModel);
                return View(viewModel);
            }

            var purchase = new Purchase
            {
                Id = viewModel.Id,
                VendorId = viewModel.VendorId,
                BranchId = viewModel.BranchId,
                PaymentMethod = viewModel.PaymentMethod,
                PurchaseDate = viewModel.PurchaseDate,
                Notes = viewModel.Notes,
                PurchaseStatus = viewModel.PurchaseStatus,
                Details = viewModel.Details.Select(d => new PurchaseDetail
                {
                    ProductId = d.ProductId,
                    Quantity = (int)d.Quantity,
                    UnitPrice = d.UnitPrice,
                    VatRate = d.VatRate,
                    SubTotal = (int)d.Quantity * d.UnitPrice,
                    VatAmount = ((int)d.Quantity * d.UnitPrice) * (d.VatRate / 100),
                    Total = ((int)d.Quantity * d.UnitPrice) * (1 + d.VatRate / 100)
                }).ToList()};

            purchase.SubTotal = purchase.Details.Sum(d => d.SubTotal);
            purchase.TotalVat = purchase.Details.Sum(d => d.VatAmount);
            purchase.TotalAmount = purchase.SubTotal + purchase.TotalVat;

            try
            {
                if (purchase.Id == 0)
                {
                    bool isCreated = await _purchaseService.CreatePurchase(purchase);
                    if (!isCreated)
                    {
                        TempData["ErrorMessage"] = "Failed to create purchase.";
                        return RedirectToAction("Purchase");
                    }
                    TempData["SuccessMessage"] = "Purchase Created Successfully";
                }
                else
                {
                    bool isUpdated = await _purchaseService.UpdatePurchase(purchase);
                    if (!isUpdated)
                    {
                        TempData["ErrorMessage"] = "Failed to update purchase.";
                        return RedirectToAction("Purchase");
                    }
                    TempData["SuccessMessage"] = "Purchase Updated Successfully";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                await PopulateDropdowns(viewModel);
                return View(viewModel);
            }

            return RedirectToAction("Purchase");
        }

        private async Task PopulateDropdowns(PurchaseViewModel viewModel)
        {
            var vendors = await _vendorService.GetAllVendor();
            var products = await _productService.GetAllProduct();
            var branches = await _branchService.GetAllBranches();

            viewModel.VendorList = new SelectList(vendors, "Id", "Name");
            viewModel.BranchList = new SelectList(branches, "Id", "Name");
            ViewBag.ProductList = new SelectList(products, "Id", "Name");
        }

        [HttpPost]
        public async Task<IActionResult> DeletePurchase(int id)
        {
            var result = await _purchaseService.DeletePurchase(id);

            if (result.Contains("Purchase deleted successfully", StringComparison.OrdinalIgnoreCase))
            {
                TempData["SuccessMessageSweet"] = result;
            }
            else
            {
                TempData["ErrorMessageSweet"] = result;
            }

            return RedirectToAction("Purchase");
        }

    }
}
