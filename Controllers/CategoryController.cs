using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.AspNetCore.Mvc;

namespace AccuStock.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Category()
        {
            var categories = await _categoryService.GetAllCategory();
            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> AddCategory(int? id)
        {
            if (id.HasValue)
            {
                var category = await _categoryService.GetCategoryById(id.Value);
                if (category == null)
                {
                    return NotFound();
                }
                return View(category);
            }

            return View(new Category());
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateCat(Category category)
        {
            if (category.ParentCategoryId == 0)
            {
                category.ParentCategoryId = null;
            }

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid input data." });
            }

            if (category.Id == 0)
            {
                bool isCreated = await _categoryService.CreateCategory(category);
                if (!isCreated)
                {
                    return Json(new { success = false, message = "A Category already exists for this SubscriptionId." });
                }
                return Json(new { success = true, message = "Category Created Successfully" });
            }
            else
            {
                bool isUpdated = await _categoryService.UpdateCategory(category);
                if (!isUpdated)
                {
                    return Json(new { success = false, message = "Category name already exists or update failed" });
                }
                return Json(new { success = true, message = "Category Updated Successfully" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCat(int id)
        {
            var result = await _categoryService.DeleteCategory(id);
            if (result.Contains("not found"))
            {
                TempData["ErrorMessage"] = result;
            }
            else
            {
                TempData["SuccessMessage"] = result;
            }
            return RedirectToAction("Category");
        }
    }
}