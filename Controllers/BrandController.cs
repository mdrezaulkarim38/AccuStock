using AccuStock.Interface;
using AccuStock.Migrations;
using AccuStock.Models;
using AccuStock.Services;
using Microsoft.AspNetCore.Mvc;

namespace AccuStock.Controllers
{
    public class BrandController : Controller
    {
        private readonly IBrandService _brandService;
        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }
        [HttpGet]
        public async Task<IActionResult> Brand()
        {
            var brands = await _brandService.GetAllBrand();
            return View(brands);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateBrand(Brand brand)
        {
            if (brand.Id == 0)
            {
                bool isCreated = await _brandService.CreateBrand(brand);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "A Brand already exists for this SubscriptionId.";
                    return RedirectToAction("Brand");
                }
                TempData["SuccessMessage"] = "Brand Created Successfully";
            }
            else
            {
                bool isUpdated = await _brandService.UpdateBrand(brand);
                if (!isUpdated)
                {
                    TempData["ErrorMessage"] = "Brand name already exists or update failed";
                    return RedirectToAction("Brand");
                }
                TempData["SuccessMessage"] = "Brand Updated Successfully";
            }

            return RedirectToAction("Brand");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var result = await _brandService.DeleteBrand(id);
            TempData["SuccessMessage"] = "Brand deleted successfully.";
            return RedirectToAction("Brand");
        }
    }
}
