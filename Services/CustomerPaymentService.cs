using AccuStock.Data;
using AccuStock.Models.ViewModels.CustomerPayment;
using AccuStock.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore;
using AccuStock.Interface;

namespace AccuStock.Services
{
    public class CustomerPaymentService : ICustomerPaymentService
    {
        private readonly AppDbContext _context;
        private readonly BaseService _baseService;
        public CustomerPaymentService(AppDbContext context, BaseService baseService)
        {
            _context = context;
            _baseService = baseService;
        }

        public async Task<List<CustomerPayment>> GetAllPayment()
        {
            var subscriptionId = _baseService.GetSubscriptionId();
            var userId = _baseService.GetUserId();
            var branchId = await _baseService.GetBranchId(subscriptionId, userId);

            var query = _context.CustomerPayments
                .Include(c => c.Branch)
                .Include(p => p.Customer)
                .Where(p => p.SubscriptionId == subscriptionId)
                .AsQueryable();

            if (branchId != 0)
            {
                query = query.Where(p => p.BranchId == branchId);
            }

            return await query.ToListAsync();
        }

        public async Task<List<CustomerPaymentViewModel>> GetCustomerSalesAsync(int customerId, DateTime startDate, DateTime endDate)
        {
            var subscriptionId = _baseService.GetSubscriptionId();
            var sales = await _context.Sales
                .Where(p => p.SubscriptionId == subscriptionId &&
                            p.PaymentMethod == 0 &&
                            p.CustomerId == customerId &&
                            p.InvoiceDate >= startDate &&
                            p.InvoiceDate <= endDate)
                .Include(p => p.Customer)
                .Select(p => new CustomerPaymentViewModel
                {
                    SaleId = p.Id,
                    SaleNo = p.InvoiceNumber!,
                    CustomerName = p.Customer!.Name!,
                    PhoneNumber = p.Customer.PhoneNumber,
                    SaleDate = p.InvoiceDate,
                    BillAmount = p.TotalAmount,
                    AmountPaid = _context.CustomerPayments
                                    .Where(cp => cp.SaleId == p.Id)
                                    .Sum(cp => (decimal?)cp.AmountPaid) ?? 0
                })
                .ToListAsync();
            return sales!;
        }

        public async Task<bool> MakePaymentAsync(CustomerPaymentRequestVM request)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var sale = await _context.Sales.FirstOrDefaultAsync(p => p.Id == request.SaleId);
                if (sale == null) throw new Exception("Sale not found");

                var subscriptionId = _baseService.GetSubscriptionId();
                var userId = _baseService.GetUserId();
                var branchId = sale.BranchId;
                var customerId = sale.CustomerId;

                var payment = new CustomerPayment
                {
                    SaleId = request.SaleId,
                    AmountPaid = request.AmountPaid,
                    PaymentDate = DateTime.Now,
                    PaymentMethod = request.PaymentMethod,
                    Notes = request.Notes,
                    SubscriptionId = subscriptionId,
                    UserId = userId,
                    CustomerId = customerId,
                    BranchId = branchId
                };
                await _context.CustomerPayments.AddAsync(payment);
                await _context.SaveChangesAsync();

                var totalPaid = _context.CustomerPayments
                    .Where(cp => cp.SaleId == sale.Id)
                    .Sum(cp => (decimal?)cp.AmountPaid) ?? 0;

                if (totalPaid >= sale.TotalAmount)
                    sale.PaymentStatus = 1; // Fully Paid
                else if (totalPaid > 0)
                    sale.PaymentStatus = 2; // Partially Paid

                _context.Sales.Update(sale);
                await _context.SaveChangesAsync();

                // Add Journal Entry
                var journal = new JournalPost
                {
                    BusinessYearId = await _baseService.GetBusinessYearId(subscriptionId),
                    BranchId = branchId,
                    VchNo = await _baseService.GenerateVchNoAsync(subscriptionId),
                    VchDate = DateTime.Now,
                    VchType = 5, // Customer Payment
                    Debit = request.AmountPaid,
                    Credit = request.AmountPaid,
                    SaleId = sale.Id,
                    UserId = userId,
                    RefNo = sale.InvoiceNumber,
                    Notes = "Customer Payment",
                    Created = DateTime.Now,
                    CustomerPaymentId = payment.Id,
                    SubscriptionId = subscriptionId
                };
                await _context.JournalPosts.AddAsync(journal);
                await _context.SaveChangesAsync();

                var journalDetails = new List<JournalPostDetail>
            {
                // Debit: Cash/Bank
                new JournalPostDetail
                {
                    BusinessYearId = journal.BusinessYearId,
                    BranchId = branchId,
                    JournalPostId = journal.Id,
                    ChartOfAccountId = 20, // Cash
                    Debit = request.AmountPaid,
                    VchNo = journal.VchNo,
                    VchDate = journal.VchDate,
                    VchType = journal.VchType,
                    Description = "Customer Payment",
                    Remarks = "Cash Received",
                    SaleId = sale.Id,
                    CustomerPaymentId = payment.Id,
                    SubscriptionId = subscriptionId,
                    CreatedAt = DateTime.Now
                },
                // Credit: Accounts Receivable
                new JournalPostDetail
                {
                    BusinessYearId = journal.BusinessYearId,
                    BranchId = branchId,
                    JournalPostId = journal.Id,
                    ChartOfAccountId = 23, // Accounts Receivable
                    Credit = request.AmountPaid,
                    VchNo = journal.VchNo,
                    VchDate = journal.VchDate,
                    VchType = journal.VchType,
                    Description = "Customer Payment",
                    Remarks = "Accounts Receivable Cleared",
                    SaleId = sale.Id,
                    CustomerPaymentId = payment.Id,
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
