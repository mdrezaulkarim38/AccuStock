using AccuStock.Models;

namespace AccuStock.Interface
{
    public interface ISettingService
    {
        Task<List<Company>> GetAllAsync();  
        Task<Company> GetByIdAsync(int id);
        Task<bool> CreateCompanyAsync(Company company);
        Task<bool> UpdateCompanyAsync(Company company);
        Task<bool> IsCompanyNameExistsAsync(string name);
        Task<Company?> GetCompanyBySubscriptionId();
        Task<List<Branch>> GetAllBranches();
        Task<bool> CreateBranch(Branch branch);

    }
}