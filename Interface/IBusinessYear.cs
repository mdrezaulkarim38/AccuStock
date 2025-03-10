using AccuStock.Models;
namespace AccuStock.Interface;

public interface IBusinessYear
{
    Task<bool> CreateBusinessYear(BusinessYear businessYear);   
}
