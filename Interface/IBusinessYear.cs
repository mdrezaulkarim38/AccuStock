using AccuStock.Models;
namespace AccuStock.Interface;

public interface IBusinessYear
{
    Task<bool> CreateBusinessYear(BusinessYear businessYear);
    Task<bool> UpdateBusinessYear(BusinessYear businessYear);
    Task<List<BusinessYear>> GetAllBusinessYear();
    Task<bool> ToggleBusinessYearStatusAsync(int busineesyearId);
}
