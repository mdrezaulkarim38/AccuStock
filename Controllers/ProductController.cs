using AccuStock.Interface;
using AccuStock.Models;
using AccuStock.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AccuStock.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly IBrandService _brandService;
    private readonly ICategoryService _categoryService;
    private readonly BaseService _baseService;
    public ProductController(IProductService productService, IBrandService brandService, ICategoryService categoryService,BaseService baseService )
    {
        _productService = productService;
        _brandService = brandService;
        _categoryService = categoryService;
        _baseService = baseService;
    }
    [HttpGet]
    public async Task<IActionResult> Product()
    {
        var products = await _productService.GetAllProduct();
        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> AddOrEditProduct(int id = 0)
    {
        ViewBag.Brands = new SelectList(await _brandService.GetAllBrand(), "Id", "Name");
        ViewBag.Categories = new SelectList(await _categoryService.GetAllCategory(), "Id", "Name");
        ViewBag.Units = new SelectList(await _baseService.GetAllUnit(), "Id", "Name");
        if (id == 0)
        {
            return View(new Product());
        }
        else
        {
            return View(await _productService.GetProductbyId(id));
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddOrEditProduct(Product product)
    {
        if(product.Id == 0)
        {
            bool isCreated = await _productService.CreateProduct(product);
            if (!isCreated)
            {
                TempData["ErrorMessage"] = "A Product already exists for this SubscriptionId.";
                return RedirectToAction("Product");
            }
            TempData["SuccessMessage"] = "Product Created Successfully";
        }
        else
        {
            bool isUpdated = await _productService.UpdateProduct(product);
            if (!isUpdated)
            {
                TempData["ErrorMessage"] = "Product name already exists or update failed";
                return RedirectToAction("Product");
            }
            TempData["SuccessMessage"] = "Product Updated Successfully";
        }
        return RedirectToAction("Product");
    }

    [HttpPost]
    public async Task<IActionResult> ToggleStatus(int productId)
    {
        bool success = await _productService.ToggleStatus(productId);

        if (!success)
        {
            TempData["ErrorMessage"] = "Product Not Found";
            return RedirectToAction("Product");
        }

        return RedirectToAction("Product");
    }
}