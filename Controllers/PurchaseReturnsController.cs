using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using AccuStock.Models.ViewModels.Purchase;
using AccuStock.Models.ViewModels.PurchaseReturn;
using AccuStock.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AccuStock.Controllers
{
    public class PurchaseReturnsController : Controller
    {
        private readonly IpurchaseReturnService _purchaseReturnService;
        private readonly IBranchService _branchService;
        private readonly AppDbContext _context;
        private readonly BaseService _baseService;

        public PurchaseReturnsController(IpurchaseReturnService purchaseReturnService, IBranchService branchService, AppDbContext context, BaseService baseService)
        {
            _purchaseReturnService = purchaseReturnService;
            _branchService = branchService;
            _context = context;
            _baseService = baseService;
        }

        // GET: PurchaseReturns/AddPurchaseReturn
        [HttpGet]
        public async Task<IActionResult> AddPurchaseReturn()
        {
            var purchases = await _purchaseReturnService.GetPurchasesForReturn();
            var branches = await _branchService.GetAllBranches();

            var viewModel = new PurchaseReturnVM
            {
                PurchaseList = new SelectList(purchases, "Id", "PurchaseNo"),
                BranchList = new SelectList(branches, "Id", "Name"),
                ReturnDate = DateTime.Now,
                Details = new List<PurchaseReturnDetailVM> { new PurchaseReturnDetailVM() }
            };
            Console.WriteLine(viewModel);
            return View(viewModel);
        }

        // POST: PurchaseReturns/AddPurchaseReturn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPurchaseReturn(PurchaseReturnVM viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(viewModel);
                return View(viewModel);
            }

            var purchaseReturn = new PurchaseReturn
            {
                PurchaseId = viewModel.PurchaseId,
                BranchId = viewModel.BranchId,
                ReturnDate = viewModel.ReturnDate,
                Notes = viewModel.Notes,
                ReturnStatus = 0,
                PurchaseReturnDetails = viewModel.Details
                    .Where(d => d.Quantity > 0)
                    .Select(d => new PurchaseReturnDetail
                    {
                        PurchaseDetailId = d.PurchaseDetailId,
                        ProductId = d.ProductId,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        VatRate = d.VatRate,
                        SubTotal = d.Quantity * d.UnitPrice,
                        VatAmount = (d.Quantity * d.UnitPrice) * (d.VatRate / 100),
                        Total = (d.Quantity * d.UnitPrice) * (1 + d.VatRate / 100),
                        Reason = d.Reason
                    }).ToList()
            };

            try
            {
                bool isCreated = await _purchaseReturnService.CreatePurchaseReturn(purchaseReturn);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "Failed to create purchase return.";
                    await PopulateDropdowns(viewModel);
                    return View(viewModel);
                }

                TempData["SuccessMessage"] = "Purchase Return Created Successfully";
                return RedirectToAction("PurchaseReturn");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                await PopulateDropdowns(viewModel);
                return View(viewModel);
            }
        }

        // GET: PurchaseReturns/PurchaseReturn
        [HttpGet]
        public async Task<IActionResult> PurchaseReturn()
        {
            var subscriptionId = _baseService.GetSubscriptionId();
            var purchaseReturns = await _context.PurchaseReturns
                .Include(pr => pr.Purchase)
                .Include(pr => pr.Vendor)
                .Where(pr => pr.SubscriptionId == subscriptionId)
                .Select(pr => new PurchaseReturnListViewModel
                {
                    Id = pr.Id,
                    ReturnNo = pr.ReturnNo!,
                    PurchaseNo = pr.Purchase!.PurchaseNo!,
                    VendorName = pr.Vendor!.Name!,
                    ReturnDate = pr.ReturnDate,
                    TotalAmount = pr.TotalAmount,
                    Status = pr.ReturnStatus == 0 ? "Pending" : pr.ReturnStatus == 1 ? "Completed" : "Cancelled"
                })
                .ToListAsync();

            return View(purchaseReturns);
        }

        // GET: PurchaseReturns/ViewPurchaseReturn/5
        [HttpGet]
        public async Task<IActionResult> ViewPurchaseReturn(int id)
        {
            var subscriptionId = _baseService.GetSubscriptionId();
            var purchaseReturn = await _context.PurchaseReturns
                .Include(pr => pr.Purchase!)
                .Include(pr => pr.Vendor!)
                .Include(pr => pr.PurchaseReturnDetails!)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(pr => pr.Id == id && pr.SubscriptionId == subscriptionId);

            if (purchaseReturn == null)
            {
                TempData["ErrorMessage"] = "Purchase return not found.";
                return RedirectToAction("PurchaseReturn");
            }

            var viewModel = new PurchaseReturnVM
            {
                Id = purchaseReturn.Id,
                PurchaseId = purchaseReturn.PurchaseId,
                BranchId = purchaseReturn.BranchId,
                ReturnDate = purchaseReturn.ReturnDate,
                Notes = purchaseReturn.Notes,
                PurchaseList = new SelectList(await _purchaseReturnService.GetPurchasesForReturn(), "Id", "PurchaseNo", purchaseReturn.PurchaseId),
                BranchList = new SelectList(await _branchService.GetAllBranches(), "Id", "Name", purchaseReturn.BranchId),
                Details = purchaseReturn.PurchaseReturnDetails!.Select(d => new PurchaseReturnDetailVM
                {
                    PurchaseDetailId = d.PurchaseDetailId,
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    VatRate = d.VatRate
                }).ToList()
            };

            return View(viewModel);
        }

        // GET: Purchase/GetPurchaseDetails/5
        [HttpGet]
        public async Task<IActionResult> GetPurchaseDetails(int id)
        {
            var subscriptionId = _baseService.GetSubscriptionId();
            var purchase = await _context.Purchases
                .Include(p => p.Details!)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(p => p.Id == id && p.SubscriptionId == subscriptionId);

            if (purchase == null)
            {
                return NotFound();
            }

            var details = purchase.Details!.Select(d => new
            {
                d.Id,
                d.ProductId,
                ProductName = d.Product!.Name,
                d.Quantity,
                d.UnitPrice,
                d.VatRate,
                branchId = purchase.BranchId,
                Notes = purchase.Notes,
                ReturnedQuantity = _context.PurchaseReturnDetails
                    .Where(prd => prd.PurchaseDetailId == d.Id)
                    .Sum(prd => prd.Quantity),
                AvailableToReturn = d.Quantity - _context.PurchaseReturnDetails
                    .Where(prd => prd.PurchaseDetailId == d.Id)
                    .Sum(prd => prd.Quantity)
            }).Where(d => d.AvailableToReturn > 0).ToList();

            return Json(new { details });
        }

        private async Task PopulateDropdowns(PurchaseReturnVM viewModel)
        {
            var purchases = await _purchaseReturnService.GetPurchasesForReturn();
            var branches = await _branchService.GetAllBranches();
            viewModel.PurchaseList = new SelectList(purchases, "Id", "PurchaseNo", viewModel.PurchaseId);
            viewModel.BranchList = new SelectList(branches, "Id", "Name", viewModel.BranchId);
        }
    }
}