﻿using AccuStock.Models;
using AccuStock.Models.ViewModels.All_TransAction;
using AccuStock.Models.ViewModels.GeneralLedger;

namespace AccuStock.Interface
{
    public interface ITransactionService
    {
        Task<List<AllTransAction>> GetAllTransAction(DateTime? startDate, DateTime? endDate, int? branchId, int? chartOfAccountId);
    }
}
