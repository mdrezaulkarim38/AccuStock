using Microsoft.EntityFrameworkCore;
using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using System.Security.Claims;

namespace AccuStock.Services
{
    public class SettingService : ISettingService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SettingService(AppDbContext context, 
                            IWebHostEnvironment webHostEnvironment, 
                            IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<List<Company>> GetAllAsync()
        {
            return await _context.Companies.ToListAsync();
        }

        public async Task<Company> GetByIdAsync(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                throw new KeyNotFoundException($"Company with Id {id} not found.");
            }
            return company;
        }

        public async Task<bool> IsCompanyNameExistsAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;
            
            return await _context.Companies.AnyAsync(c => c.Name == name);
        }

        public async Task<bool> CreateCompanyAsync(Company company)
        {
            try
            {
                if (company == null)
                    throw new ArgumentNullException(nameof(company));

                var subscriptionIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")?.Value;
                if (subscriptionIdClaim != null)
                {
                    var subscriptionId = int.Parse(subscriptionIdClaim);
                    var existingCompany = await _context.Companies
                        .FirstOrDefaultAsync(c => c.SubscriptionId == subscriptionId);

                    if (existingCompany != null)
                    {
                        return false;
                    }
                    company.SubscriptionId = subscriptionId;
                }

                if (company.LogoImage != null)
                {
                    company.LogoPath = await UploadFileAsync(company.LogoImage);
                }
                await _context.AddAsync(company);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<bool> UpdateCompanyAsync(Company company)
        {
            try
            {
                if (company == null)
                    throw new ArgumentNullException(nameof(company));

                var existingCompany = await _context.Companies.FirstOrDefaultAsync(c => c.Id == company.Id);
                if (existingCompany == null)
                {
                    return false;
                }
                if (company.LogoImage != null)
                {
                    if (!string.IsNullOrEmpty(existingCompany.LogoPath))
                    {
                        DeleteFile(existingCompany.LogoPath);
                    }
                    existingCompany.LogoPath = await UploadFileAsync(company.LogoImage);
                }

                existingCompany.Name = company.Name;
                existingCompany.TagLine = company.TagLine;
                existingCompany.VatRegistrationNo = company.VatRegistrationNo;
                existingCompany.TinNo = company.TinNo;
                existingCompany.WebsiteLink = company.WebsiteLink;
                existingCompany.Email = company.Email;
                existingCompany.ContactNumber = company.ContactNumber;
                existingCompany.Address = company.Address;
                existingCompany.Remarks = company.Remarks;
                _context.Update(existingCompany);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

        private async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/images/{uniqueFileName}";
        }

        public async Task<Company?> GetCompanyBySubscriptionId()
        {
            var subscriptionId = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")!.Value;
            return await _context.Companies.FirstOrDefaultAsync(c => c.SubscriptionId == int.Parse(subscriptionId!));
        }

        public async Task<Branch> GetAllBranch() {
            var subscriptionId = _httpContextAccessor.HttpContext?.User.FindFirst("SubscriptionId")!.Value;
            return await _context.Branches.FirstOrDefaultAsync(b => b.SubscriptionId == int.Parse(subscriptionId!));
        }

    }
}