using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using AccuStock.Models.ViewModels.VendorPayment;
using Microsoft.EntityFrameworkCore;

namespace AccuStock.Services
{
    public class VendorPaymentService : IVendorPaymentService
    {
        private readonly AppDbContext _context;
        private readonly BaseService _baseService;
        public VendorPaymentService(AppDbContext context, BaseService baseService)
        {
            _context = context;
            _baseService = baseService;
        }

        public async Task<List<VendorPayment>> GetAllPayment()
        {
            var subscriptionId = _baseService.GetSubscriptionId();
            var userId = _baseService.GetUserId();
            var branchId = await _baseService.GetBranchId(subscriptionId, userId);

            var query = _context.VendorPayments
                .Include(c => c.Branch)
                .Include(p => p.Vendor)
                .Where(p => p.SubscriptionId == subscriptionId)
                .AsQueryable();

            if (branchId != 0)
            {
                query = query.Where(p => p.BranchId == branchId);
            }

            return await query.ToListAsync();
        }
        public async Task<List<VendorPaymentViewModel>> GetVendorPurchasesAsync(int vendorId, DateTime startDate, DateTime endDate)
        {
            var subscriptionId = _baseService.GetSubscriptionId();
            var purchases = await _context.Purchases
                .Where(p => p.SubscriptionId == subscriptionId &&
                            p.PaymentMethod == 0 &&
                            p.VendorId == vendorId &&
                            p.PurchaseDate >= startDate &&
                            p.PurchaseDate <= endDate)
                .Include(p => p.Vendor)
                .Select(p => new VendorPaymentViewModel
                {
                    PurchaseId = p.Id,
                    PurchaseNo = p.PurchaseNo!,
                    VendorName = p.Vendor!.Name!,
                    PhoneNumber = p.Vendor.Contact!,
                    PurchaseDate = p.PurchaseDate,
                    BillAmount = p.TotalAmount,
                    AmountPaid = _context.VendorPayments
                                    .Where(vp => vp.PurchaseId == p.Id)
                                    .Sum(vp => (decimal?)vp.AmountPaid) ?? 0
                })
                .ToListAsync();
            return purchases!;
        }

        public async Task<bool> MakePaymentAsync(VendorPaymentRequestVM request)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var purchase = await _context.Purchases.FirstOrDefaultAsync(p => p.Id == request.PurchaseId);
                if (purchase == null) throw new Exception("Purchase not found");

                var subscriptionId = _baseService.GetSubscriptionId();
                var userId = _baseService.GetUserId();
                var branchId = purchase.BranchId;
                var vendorId = purchase.VendorId;

                var payment = new VendorPayment
                {
                    PurchaseId = request.PurchaseId,
                    AmountPaid = request.AmountPaid,
                    PaymentDate = DateTime.Now,
                    PaymentMethod = request.PaymentMethod,
                    Notes = request.Notes,
                    SubscriptionId = subscriptionId,
                    UserId = userId,
                    VendorId = vendorId,
                    BranchId = branchId
                };
                await _context.VendorPayments.AddAsync(payment);
                await _context.SaveChangesAsync();

                var totalPaid = _context.VendorPayments
                    .Where(vp => vp.PurchaseId == purchase.Id)
                    .Sum(vp => (decimal?)vp.AmountPaid) ?? 0;

                if (totalPaid >= purchase.TotalAmount)
                    purchase.PurchaseStatus = 1; // Fully Paid
                else if (totalPaid > 0)
                    purchase.PurchaseStatus = 2; // Partially Paid

                _context.Purchases.Update(purchase);
                await _context.SaveChangesAsync();

                // Add Journal Entry
                var journal = new JournalPost
                {
                    BusinessYearId = await _baseService.GetBusinessYearId(subscriptionId),
                    BranchId = branchId,
                    VchNo = await _baseService.GenerateVchNoAsync(subscriptionId),
                    VchDate = DateTime.Now,
                    VchType = 4, // Payment
                    Debit = request.AmountPaid,
                    Credit = request.AmountPaid,
                    PurchaseId = purchase.Id,
                    UserId = userId,
                    RefNo = purchase.PurchaseNo,
                    Notes = "Vendor Payment",
                    Created = DateTime.Now,
                    VendorPaymentId = payment.Id,
                    SubscriptionId = subscriptionId
                };
                await _context.JournalPosts.AddAsync(journal);
                await _context.SaveChangesAsync();

                var journalDetails = new List<JournalPostDetail>
            {
                // Debit: Accounts Payable
                new JournalPostDetail
                {
                    BusinessYearId = journal.BusinessYearId,
                    BranchId = branchId,
                    JournalPostId = journal.Id,
                    ChartOfAccountId = 21, // Accounts Payable
                    Debit = request.AmountPaid,
                    VchNo = journal.VchNo,
                    VchDate = journal.VchDate,
                    VchType = journal.VchType,
                    Description = "Vendor Payment",
                    Remarks = "Accounts Payable Cleared",
                    PurchaseId = purchase.Id,
                    VendorPaymentId = payment.Id,
                    SubscriptionId = subscriptionId,
                    CreatedAt = DateTime.Now
                },
                // Credit: Cash/Bank
                new JournalPostDetail
                {
                    BusinessYearId = journal.BusinessYearId,
                    BranchId = branchId,
                    JournalPostId = journal.Id,
                    ChartOfAccountId = 20, // Cash
                    Credit = request.AmountPaid,
                    VchNo = journal.VchNo,
                    VchDate = journal.VchDate,
                    VchType = journal.VchType,
                    Description = "Vendor Payment",
                    Remarks = "Cash Paid",
                    PurchaseId = purchase.Id,
                    VendorPaymentId = payment.Id,
                    SubscriptionId = subscriptionId,
                    CreatedAt = DateTime.Now
                }
            };
                await _context.JournalPostDetails.AddRangeAsync(journalDetails);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}
