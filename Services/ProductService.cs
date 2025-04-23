using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Migrations;
using AccuStock.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AccuStock.Services
{
    public class ProductService: IProductService
    {
        public readonly AppDbContext _context;
        public readonly BaseService _baseService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductService(AppDbContext context, BaseService baseService, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _baseService = baseService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<List<Product>> GetAllProduct()
        {
            var subscriptionIdClaim = _baseService.GetSubscriptionId();
            return await _context.Products
                .Where(c => c.SubscriptionId == subscriptionIdClaim)
                .Include(c => c.Category)
                .Include(c => c.Brand)
                .Include(c => c.Unit)
                .ToListAsync();
        }

        public async Task<Product?> GetProductbyId(int Id)
        {
            var subscriptionIdClaim = _baseService.GetSubscriptionId();
            return await _context.Products
                .FirstOrDefaultAsync(c => c.Id == Id && c.SubscriptionId == subscriptionIdClaim);
        }

        public async Task<bool> CreateProduct(Product product)
        {
            try
            {
                var subscriptionIdClaim = _baseService.GetSubscriptionId();
                product.SubscriptionId = subscriptionIdClaim;

                if (product.ProductImage != null)
                {
                    product.ImagePath = await UploadFileAsync(product.ProductImage);
                }
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            try
            {
                var subscriptionIdClaim = _baseService.GetSubscriptionId();
                var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.SubscriptionId == subscriptionIdClaim && p.Id == product.Id);

                if (existingProduct == null)
                {
                    return false;
                }

                if(product.ProductImage!= null)
                {
                    if (!string.IsNullOrEmpty(existingProduct.ImagePath))
                    {
                        DeleteFile(existingProduct.ImagePath);
                    }
                    existingProduct.ImagePath = await UploadFileAsync(product.ProductImage);
                }

                existingProduct.Name = product.Name;
                existingProduct.UnitId = product.UnitId;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.BrandId = product.BrandId;
                existingProduct.Code = product.Code;

                _context.Update(existingProduct);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException("Unsupported file type.");

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/product");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var sanitizedFileName = Path.GetFileName(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}_{sanitizedFileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/images/product/{uniqueFileName}";
        }
        private void DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        public async Task<bool> ToggleStatus(int productId)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                    return false;

                product.Status = !product.Status;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
