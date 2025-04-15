using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models.ViewModels.All_TransAction;
using Microsoft.EntityFrameworkCore;

namespace AccuStock.Services;
public class TransactionService : ITransactionService
{
    private readonly BaseService _baseService;
    private readonly AppDbContext _context;

    public TransactionService(AppDbContext context, BaseService baseService)
    {
        _context = context;
        _baseService = baseService;
    }

    public async Task<List<AllTransAction>> GetAllTransAction(DateTime? startDate, DateTime? endDate, int? branchId, int? chartOfAccountId)
    {
        var query = _context.JournalPosts
            .Include(j => j.JournalPostDetails)
            .Include(j => j.Branch)
            .Where(j => j.SubscriptionId == _baseService.GetSubscriptionId())
            .AsQueryable();

        if (startDate != null && endDate != null)
        {
            query = query.Where(j => j.VchDate >= startDate && j.VchDate <= endDate);
        }

        if (branchId != null)
        {
            query = query.Where(j => j.BranchId == branchId);
        }

        if(chartOfAccountId != null)
        {
            query = query.Where(j => j.JournalPostDetails.Any(d => d.ChartOfAccountId == chartOfAccountId));
        }

        var groupData = await query
         .SelectMany(j => j.JournalPostDetails.Select(d => new AllTransAction
         {
             VchNo = j.VchNo,
             VchDate = j.VchDate.ToString(),
             BranchName = j.Branch!.Name,
             VchType = j.VchType!.Value.ToString(),
             Amount = Convert.ToDecimal(d.Debit ?? d.Credit),
             Description = d.Description,
             Referance = j.RefNo,
             Notes = j.Notes
         }))
         .ToListAsync();
        return groupData;
    }


    //public Task<List<AllTransAction>> GetAllTransaction()
    //{
    //    var subscriptionId = _baseService.GetSubscriptionId();

    //    var data = _context.JournalPosts
    //        .Where(j => j.SubscriptionId == subscriptionId)
    //        .OrderByDescending(j => j.VchDate)
    //        .Take(50)
    //        .Select(j => new
    //        {
    //            j.VchNo,
    //            j.VchDate,
    //            BranchName = j.Branch!.Name,
    //            VchType = j.VchType!.Value.ToString(),
    //            j.RefNo,
    //            j.Notes,
    //            Details = j.JournalPostDetails
    //        })
    //        .AsEnumerable()
    //        .Select(j => new AllTransAction
    //        {
    //            VchNo = j.VchNo,
    //            VchDate = j.VchDate.ToString(),
    //            BranchName = j.BranchName,
    //            VchType = j.VchType,
    //            Amount = j.Details.Sum(d => Convert.ToDecimal(d.Debit ?? d.Credit)),
    //            Description = j.Details.FirstOrDefault()?.Description ?? "",
    //            Referance = j.RefNo,
    //            Notes = j.Notes
    //        })
    //        .ToList();

    //    return Task.FromResult(data);
    //}

}
