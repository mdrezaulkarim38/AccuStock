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

            if(category.ParentCategoryId == null){
                category.IsParent = 1;
            }

            if (category.Id == 0)
            {
                bool isCreated = await _categoryService.CreateCategory(category);
                if (!isCreated)
                {
                    TempData["ErrorMessageSweet"] = "Unsuccessfull to Create";
                }
                else{
                    TempData["SuccessMessageSweet"] = "Category Created";
                }
            }
            else
            {
                bool isUpdated = await _categoryService.UpdateCategory(category);
                if (!isUpdated)
                {
                    TempData["ErrorMessageSweet"] = "Category name already exists or update failed";
                }
                else
                {
                    TempData["SuccessMessageSweet"] = "Category Updated Successfully";
                }
            }
            return RedirectToAction("Category"); 
        }

        [HttpPost]

        public async Task<IActionResult> DeleteCat(int id)
        {
            var result = await _categoryService.DeleteCategory(id);
            if (result.Contains("Cannot delete category! Child Already under this.")) 
            {
                TempData["ErrorMessageSweet"] = "Unsuccessfull";
            }
            if (result.Contains("Cannot delete category. It is associated with existing products."))
            {
                TempData["ErrorMessageSweet"] = "Cannot delete category. It is associated with existing products.";
            }
            else
            {
                TempData["SuccessMessageSweet"] = "Successfully Deleted";
            }
            return RedirectToAction("Category");
        }
    }
}