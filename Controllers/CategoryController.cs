using AccuStock.Interface;
using AccuStock.Models;
using AccuStock.Services;
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
        public async Task<IActionResult> AddCategory()
        {
            var categories = await _categoryService.GetAllCategory();
            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Category()
        {
            var categories = await _categoryService.GetAllCategory();
            return View(categories);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateCat(Category category)
        {
            if(category.ParentCategoryId == 0)
            {
                category.ParentCategoryId = null;
            }
            if(category.Id == 0)
            {
                bool isCreated = await _categoryService.CreateCategory(category);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "A Category already exists for this SubscriptionId.";
                    return RedirectToAction("Category");
                }
                TempData["SuccessMessage"] = "Category Created Successfully";
            }
            else
            {
                bool isUpdated = await _categoryService.UpdateCategory(category);
                if (!isUpdated)
                {
                    TempData["ErrorMessage"] = "Category name already exists or update failed";
                    return RedirectToAction("Category");
                }
                TempData["SuccessMessage"] = "Category Updated Successfully";
            }
            return RedirectToAction("Category");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCat(int id)
        {
            var result = await _categoryService.DeleteCategory(id);
            TempData["SuccessMessage"] = "Category deleted successfully.";
            return RedirectToAction("Category");
        }

    }
}
