using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models.ViewModels.SaleReturn;
using AccuStock.Models;
using AccuStock.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AccuStock.Controllers
{
    public class SaleReturnsController : Controller
    {
        private readonly ISaleReturnService _saleReturnService;
        private readonly IBranchService _branchService;
        private readonly AppDbContext _context;
        private readonly BaseService _baseService;

        public SaleReturnsController(ISaleReturnService saleReturnService, IBranchService branchService, AppDbContext context, BaseService baseService)
        {
            _saleReturnService = saleReturnService;
            _branchService = branchService;
            _context = context;
            _baseService = baseService;
        }

        // GET: SaleReturns/AddSaleReturn
        [HttpGet]
        public async Task<IActionResult> AddSaleReturn()
        {
            var sales = await _saleReturnService.GetSalesForReturn();
            var branches = await _branchService.GetAllBranches();

            var viewModel = new SaleReturnVM
            {
                SaleList = new SelectList(sales, "Id", "SaleNo"),
                BranchList = new SelectList(branches, "Id", "Name"),
                ReturnDate = DateTime.Now,
                Details = new List<SaleReturnDetailVM> { new SaleReturnDetailVM() }
            };

            return View(viewModel);
        }

        // POST: SaleReturns/AddSaleReturn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSaleReturn(SaleReturnVM viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(viewModel);
                return View(viewModel);
            }

            var saleReturn = new SaleReturn
            {
                SaleId = viewModel.SaleId,
                BranchId = viewModel.BranchId,
                ReturnDate = viewModel.ReturnDate,
                Notes = viewModel.Notes,
                ReturnStatus = 0,
                SaleReturnDetails = viewModel.Details
                    .Where(d => d.Quantity > 0)
                    .Select(d => new SaleReturnDetail
                    {
                        SaleDetailId = d.SaleDetailId,
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
                bool isCreated = await _saleReturnService.CreateSaleReturn(saleReturn);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "Failed to create sale return.";
                    await PopulateDropdowns(viewModel);
                    return View(viewModel);
                }

                TempData["SuccessMessage"] = "Sale Return Created Successfully";
                return RedirectToAction("SaleReturn");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                await PopulateDropdowns(viewModel);
                return View(viewModel);
            }
        }

        // GET: SaleReturns/SaleReturn
        [HttpGet]
        public async Task<IActionResult> SaleReturn()
        {
            var subscriptionId = _baseService.GetSubscriptionId();
            var saleReturns = await _context.SaleReturns
                .Include(sr => sr.Sale)
                .Include(sr => sr.Customer)
                .Where(sr => sr.SubscriptionId == subscriptionId)
                .Select(sr => new SaleReturnListViewModel
                {
                    Id = sr.Id,
                    ReturnNo = sr.ReturnNo!,
                    SaleNo = sr.Sale!.InvoiceNumber!,
                    CustomerName = sr.Customer!.Name!,
                    ReturnDate = sr.ReturnDate,
                    TotalAmount = sr.TotalAmount,
                    Status = sr.ReturnStatus == 0 ? "Pending" : sr.ReturnStatus == 1 ? "Completed" : "Cancelled"
                })
                .ToListAsync();

            return View(saleReturns);
        }

        // GET: SaleReturns/ViewSaleReturn/5
        [HttpGet]
        public async Task<IActionResult> ViewSaleReturn(int id)
        {
            var subscriptionId = _baseService.GetSubscriptionId();
            var saleReturn = await _context.SaleReturns
                .Include(sr => sr.Sale!)
                .Include(sr => sr.Customer!)
                .Include(sr => sr.SaleReturnDetails!)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(sr => sr.Id == id && sr.SubscriptionId == subscriptionId);

            if (saleReturn == null)
            {
                TempData["ErrorMessage"] = "Sale return not found.";
                return RedirectToAction("SaleReturn");
            }

            var viewModel = new SaleReturnVM
            {
                Id = saleReturn.Id,
                SaleId = saleReturn.SaleId,
                BranchId = saleReturn.BranchId,
                ReturnDate = saleReturn.ReturnDate,
                Notes = saleReturn.Notes,
                SaleList = new SelectList(await _saleReturnService.GetSalesForReturn(), "Id", "SaleNo", saleReturn.SaleId),
                BranchList = new SelectList(await _branchService.GetAllBranches(), "Id", "Name", saleReturn.BranchId),
                Details = saleReturn.SaleReturnDetails!.Select(d => new SaleReturnDetailVM
                {
                    SaleDetailId = d.SaleDetailId,
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    VatRate = d.VatRate
                }).ToList()
            };
            return View(viewModel);
        }

        // GET: SaleReturns/GetSaleDetails/5
        [HttpGet]
        public async Task<IActionResult> GetSaleDetails(int id)
        {
            var subscriptionId = _baseService.GetSubscriptionId();
            var sale = await _context.Sales
                .Include(s => s.SaleDetails!)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(s => s.Id == id && s.SubscriptionId == subscriptionId);

            if (sale == null)
            {
                return NotFound();
            }

            var details = sale.SaleDetails!.Select(d => new
            {
                d.Id,
                d.ProductId,
                ProductName = d.Product!.Name,
                d.Quantity,
                d.UnitPrice,
                d.VatRate,
                branchId = sale.BranchId,
                Notes = sale.Notes,
                ReturnedQuantity = _context.SaleReturnDetails
                    .Where(srd => srd.SaleDetailId == d.Id)
                    .Sum(srd => srd.Quantity),
                AvailableToReturn = d.Quantity - _context.SaleReturnDetails
                    .Where(srd => srd.SaleDetailId == d.Id)
                    .Sum(srd => srd.Quantity)
            }).Where(d => d.AvailableToReturn > 0).ToList();

            return Json(new { details });
        }

        private async Task PopulateDropdowns(SaleReturnVM viewModel)
        {
            var sales = await _saleReturnService.GetSalesForReturn();
            var branches = await _branchService.GetAllBranches();
            viewModel.SaleList = new SelectList(sales, "Id", "SaleNo", viewModel.SaleId);
            viewModel.BranchList = new SelectList(branches, "Id", "Name", viewModel.BranchId);
        }
    }
}
