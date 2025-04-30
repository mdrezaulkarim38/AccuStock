using AccuStock.Interface;
using AccuStock.Models;
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
        


        [HttpPost]
        public async Task<IActionResult> AddOrEditPurchase(Purchase purchase)
        {
            if (purchase.Id == 0)
            {
                bool isCreated = await _purchaseService.CreatePurchase(purchase);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "A purchase already exists for this SubscriptionId.";
                    return RedirectToAction("Purchase");
                }
                TempData["SuccessMessage"] = "Purchase Created Successfully";
            }
            else
            {
                bool isUpdated = await _purchaseService.UpdatePurchase(purchase);
                if (!isUpdated)
                {
                    TempData["ErrorMessage"] = "Purchase already exists or update failed";
                    return RedirectToAction("Purchase");
                }
                TempData["SuccessMessage"] = "Purchase Updated Successfully";
            }
            return RedirectToAction("Purchase");
        }
    }
}
