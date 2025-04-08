using AccuStock.Models.ViewModels.GeneralLedger;

namespace AccuStock.Interface;
public interface IGLedger
{
    Task<List<GLedger>> GetGLedger(DateTime? startDate, DateTime? endDate, int? branchId, int? chartOfAccountId);
    Task<List<AGLedger>> GetAGLedgersList();

}