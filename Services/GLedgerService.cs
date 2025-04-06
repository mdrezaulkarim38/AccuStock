using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using AccuStock.Models.ViewModels.GeneralLedger;
using Microsoft.EntityFrameworkCore;

namespace AccuStock.Services;
public class GLedgerService : IGLedger
{
    private readonly AppDbContext _context;
    private readonly BaseService _baseService;

    public GLedgerService(AppDbContext context, BaseService baseService)
    {
        _context = context;
        _baseService = baseService;
    }

    public async Task<List<GLedger>> GetAllGLedger()
    {
        // Query JournalPostDetails and include related ChartOfAccount and JournalPost
        var glEntries = await _context.JournalPostDetails
            .Include(jpd => jpd.ChartOfAccount)    // Include ChartOfAccount
            .Include(jpd => jpd.JournalPost)      // Include JournalPost
            .Select(jpd => new GLedger
            {
                // Map the necessary data to the GLedger view model
                ChartOfAccount = new ChartOfAccount
                {
                    // Assuming you want to display the Account Head Name
                    Name = jpd.ChartOfAccount!.Name,
                    AccountCode = jpd.ChartOfAccount.AccountCode
                },
                JournalPost = new JournalPost
                {
                    // JournalPost now includes relevant data, like created date
                    Id = jpd.JournalPost!.Id,
                    Created = jpd.JournalPost.Created,
                },
                JournalPostDetail = new JournalPostDetail
                {
                    // Map the details from JournalPostDetail
                    Id = jpd.Id,
                    Debit = jpd.Debit,
                    Credit = jpd.Credit,
                    VchNo = jpd.VchNo,
                    VchDate = jpd.VchDate,
                    Remarks = jpd.Remarks,
                    ChqNo = jpd.ChqNo,
                    ChqDate = jpd.ChqDate,
                    Description = jpd.Description // Map the Description from JournalPostDetail
                }
            })
            .Where(jpd => jpd.ChartOfAccount != null)   // Ensure ChartOfAccount exists
            .ToListAsync();

        // Return the populated list of GLedger objects
        return glEntries;
    }
}