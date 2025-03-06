using AccuStock.Models;

namespace AccuStock.Interface
{
    public interface IBranchService
    {
        Task<List<Branch>> GetAllBranches();
        Task<bool> CreateBranch(Branch branch);
        Task<bool> UpdateBranch(Branch branch);
    }
}