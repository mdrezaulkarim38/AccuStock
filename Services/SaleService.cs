using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.EntityFrameworkCore;

namespace AccuStock.Services
{
    public class SaleService : ISaleService
    {
        private readonly AppDbContext _context;
        private readonly BaseService _baseService;
        public SaleService(AppDbContext context, BaseService baseService)
        {
            _context = context;
            _baseService = baseService;
        }
        public async Task<List<Sale>> GetAllSale()
        {
            var subscriptionId = _baseService.GetSubscriptionId();
            var userId = _baseService.GetUserId();
            var branchId = await _baseService.GetBranchId(subscriptionId, userId);

            //return await _context.Sales
            //    .Where(c => c.SubscriptionId == subscriptionId && c.BranchId == branchId)
            //    .Include(c => c.Customer)
            //    .Include(c => c.Branch)
            //    .ToListAsync();

            var query = _context.Sales.Include(s => s.Customer).Include(s => s.Branch).Where(s => s.SubscriptionId == subscriptionId).AsQueryable();
            return await query.ToListAsync();
        }
        public async Task<Sale?> GetSalebyId(int id)
        {
            if (id <= 0)
                return null;

            try
            {
                var subscriptionId = _baseService.GetSubscriptionId();
                //var userId = _baseService.GetUserId();
                //var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                return await _context.Sales
                    .Include(s => s.Customer)
                    .Include(s => s.Branch)
                    .Include(s => s.SaleDetails)
                    .Where(s => s.SubscriptionId == subscriptionId && s.Id == id)
                    .FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                // Log the error (use your logging framework)
                return null;
            }
        }
        public async Task<int> GetSalebyInvNum(string invoiceNumber)
        {
            var subscriptionId = _baseService.GetSubscriptionId();
            var userId = _baseService.GetUserId();
            var branchId = await _baseService.GetBranchId(subscriptionId, userId);

            var sale = await _context.Sales
                .Where(s => s.InvoiceNumber == invoiceNumber
                            && s.SubscriptionId == subscriptionId
                            && s.BranchId == branchId)
                .FirstOrDefaultAsync();
            return sale?.Id ?? 0;
        }

        public async Task<bool> CreateSale(Sale sale)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var subscriptionId = _baseService.GetSubscriptionId();
                var userId = _baseService.GetUserId();
                var branchId = sale.BranchId;

                sale.SubscriptionId = subscriptionId;
                sale.InvoiceNumber = await GenerateInvoiceNo();
                if (sale.SaleDetails == null || !sale.SaleDetails.Any())
                {
                    throw new ArgumentException("SaleDetails cannot be null or empty.");
                }
                foreach (var detail in sale.SaleDetails!)
                {
                    detail.Sale = sale;
                    detail.SubTotal = detail.Quantity * detail.UnitPrice;
                    detail.VatAmount = detail.SubTotal * (detail.VatRate / 100);
                    detail.Total = detail.SubTotal + detail.VatAmount;
                    detail.SubscriptionId = subscriptionId;
                }

                sale.SubTotal = sale.SaleDetails.Sum(d => d.SubTotal);
                sale.TotalVat = sale.SaleDetails.Sum(d => d.VatAmount);
                sale.TotalAmount = sale.SubTotal + sale.TotalVat;

                await _context.Sales.AddAsync(sale);
                await _context.SaveChangesAsync();

                // Record Product Stock Out
                foreach (var detail in sale.SaleDetails)
                {
                    var currentStock = await _context.ProductStocks
                        .Where(ps => ps.ProductId == detail.ProductId && ps.SubscriptionId == subscriptionId)
                        .SumAsync(ps => ps.QuantityIn - ps.QuantityOut);

                    if (currentStock < detail.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock for product ID {detail.ProductId}. Available: {currentStock}, Requested: {detail.Quantity}");
                    }
                    var stock = new ProductStock
                    {
                        ProductId = detail.ProductId,
                        Date = sale.InvoiceDate,
                        QuantityOut = detail.Quantity,
                        QuantityIn = 0,
                        SourceType = "Sale",
                        ReferenceNo = sale.InvoiceNumber ?? "",
                        SourceId = sale.Id,
                        Remarks = "Stock reduced from sale",
                        SubscriptionId = subscriptionId
                    };
                    await _context.ProductStocks.AddAsync(stock);
                }
                await _context.SaveChangesAsync();
                // Journal Post
                var journal = new JournalPost
                {
                    BusinessYearId = await _baseService.GetBusinessYearId(subscriptionId),
                    BranchId = branchId,
                    VchNo = await _baseService.GenerateVchNoAsync(subscriptionId),
                    VchDate = sale.InvoiceDate,
                    VchType = 5, // Sale VCH Type (define as needed)
                    Debit = sale.TotalAmount,
                    Credit = sale.TotalAmount,
                    SaleId = sale.Id,
                    UserId = userId,
                    RefNo = sale.InvoiceNumber,
                    Notes = "Sales Entry",
                    Created = DateTime.Now,
                    SubscriptionId = subscriptionId,
                };

                await _context.JournalPosts.AddAsync(journal);
                await _context.SaveChangesAsync();

                var journalDetails = new List<JournalPostDetail>();
                if (sale.PaymentMethod == 0) // On Credit
                {
                    journalDetails.Add(new JournalPostDetail
                    {
                        ChartOfAccountId = 23, // Accounts Receivable
                        Debit = sale.TotalAmount,
                        Description = "Credit Sale - Receivable",
                        Remarks = "Accounts Receivable",
                    });
                }
                else if (sale.PaymentMethod == 1) // On Cash
                {
                    journalDetails.Add(new JournalPostDetail
                    {
                        ChartOfAccountId = 20, // Cash in Hand
                        Debit = sale.TotalAmount,
                        Description = "Cash Sale",
                        Remarks = "Cash received from sale",
                    });
                }
                // Sales Revenue Credit
                journalDetails.Add(new JournalPostDetail
                {
                    ChartOfAccountId = 24, // Sales Revenue
                    Credit = sale.TotalAmount,
                    Description = "Sales Revenue",
                    Remarks = "Credit revenue for sale"
                });
                //decimal totalCOGS = sale.SaleDetails.Sum(d => d.Quantity * (d.Product?.CostPrice ?? 0)); // You must have CostPrice in Product
                journalDetails.Add(new JournalPostDetail
                {
                    ChartOfAccountId = 25, // Cost of Goods Sold
                    Debit = sale.TotalAmount,
                    Description = "COGS Entry",
                    Remarks = "Record expense for COGS"
                });

                journalDetails.Add(new JournalPostDetail
                {
                    ChartOfAccountId = 22, // Inventory
                    Credit = sale.TotalAmount,
                    Description = "Inventory Reduction",
                    Remarks = "Reduce inventory for sold items"
                });

                // Finalize Journal Details
                foreach (var jd in journalDetails)
                {
                    jd.JournalPostId = journal.Id;
                    jd.BranchId = branchId;
                    jd.VchNo = journal.VchNo;
                    jd.VchDate = journal.VchDate;
                    jd.VchType = journal.VchType;
                    jd.BusinessYearId = journal.BusinessYearId;
                    jd.SaleId = sale.Id;
                    jd.SubscriptionId = subscriptionId;
                    jd.CreatedAt = DateTime.Now;
                }

                await _context.JournalPostDetails.AddRangeAsync(journalDetails);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"CreateSale Error: {ex.Message}");
                throw new InvalidOperationException("An unexpected error occurred while creating the sale.", ex);
            }
        }

        public async Task<bool> UpdateSale(int saleId, Sale updatedSale)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var subscriptionId = _baseService.GetSubscriptionId();
                var userId = _baseService.GetUserId();
                var branchId = updatedSale.BranchId;

                // Fetch the existing sale with its details
                var existingSale = await _context.Sales
                    .Include(s => s.SaleDetails)
                    .FirstOrDefaultAsync(s => s.Id == saleId && s.SubscriptionId == subscriptionId);

                if (existingSale == null)
                {
                    throw new ArgumentException("Sale not found.");
                }
                // Update basic sale properties
                existingSale.BranchId = updatedSale.BranchId;
                existingSale.InvoiceDate = updatedSale.InvoiceDate;
                existingSale.PaymentMethod = updatedSale.PaymentMethod;
                existingSale.CustomerId = updatedSale.CustomerId;
                existingSale.Notes = updatedSale.Notes;
                existingSale.InvoiceDate = updatedSale.InvoiceDate;

                // Validate SaleDetails
                if (updatedSale.SaleDetails == null || !updatedSale.SaleDetails.Any())
                {
                    throw new ArgumentException("SaleDetails cannot be null or empty.");
                }

                // Remove old SaleDetails and associated stock entries
                var oldDetails = existingSale.SaleDetails.ToList();
                foreach (var oldDetail in oldDetails)
                {
                    var stock = await _context.ProductStocks
                        .FirstOrDefaultAsync(ps => ps.SourceId == existingSale.Id && ps.ProductId == oldDetail.ProductId && ps.SourceType == "Sale");
                    if (stock != null)
                    {
                        _context.ProductStocks.Remove(stock);
                    }
                }
                _context.SaleDetails.RemoveRange(oldDetails);

                // Add new SaleDetails
                foreach (var detail in updatedSale.SaleDetails)
                {
                    detail.SaleId = existingSale.Id;
                    detail.SubTotal = detail.Quantity * detail.UnitPrice;
                    detail.VatAmount = detail.SubTotal * (detail.VatRate / 100);
                    detail.Total = detail.SubTotal + detail.VatAmount;
                    detail.SubscriptionId = subscriptionId;
                }

                // Update sale totals
                existingSale.SubTotal = updatedSale.SaleDetails.Sum(d => d.SubTotal);
                existingSale.TotalVat = updatedSale.SaleDetails.Sum(d => d.VatAmount);
                existingSale.TotalAmount = existingSale.SubTotal + existingSale.TotalVat;

                // Add new SaleDetails to context
                await _context.SaleDetails.AddRangeAsync(updatedSale.SaleDetails);

                // Update Product Stock
                foreach (var detail in updatedSale.SaleDetails)
                {
                    var currentStock = await _context.ProductStocks
                        .Where(ps => ps.ProductId == detail.ProductId && ps.SubscriptionId == subscriptionId)
                        .SumAsync(ps => ps.QuantityIn - ps.QuantityOut);

                    if (currentStock < detail.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock for product ID {detail.ProductId}. Available: {currentStock}, Requested: {detail.Quantity}");
                    }
                    var stock = new ProductStock
                    {
                        ProductId = detail.ProductId,
                        Date = existingSale.InvoiceDate,
                        QuantityOut = detail.Quantity,
                        QuantityIn = 0,
                        SourceType = "Sale",
                        ReferenceNo = existingSale.InvoiceNumber ?? "",
                        SourceId = existingSale.Id,
                        Remarks = "Stock reduced from updated sale",
                        SubscriptionId = subscriptionId
                    };
                    await _context.ProductStocks.AddAsync(stock);
                }

                // Remove old journal entries
                var oldJournal = await _context.JournalPosts
                    .FirstOrDefaultAsync(j => j.SaleId == existingSale.Id && j.SubscriptionId == subscriptionId);
                if (oldJournal != null)
                {
                    var oldJournalDetails = await _context.JournalPostDetails
                        .Where(jd => jd.JournalPostId == oldJournal.Id)
                        .ToListAsync();
                    _context.JournalPostDetails.RemoveRange(oldJournalDetails);
                    _context.JournalPosts.Remove(oldJournal);
                }

                // Create new journal entry
                var journal = new JournalPost
                {
                    BusinessYearId = await _baseService.GetBusinessYearId(subscriptionId),
                    BranchId = branchId,
                    VchNo = await _baseService.GenerateVchNoAsync(subscriptionId),
                    VchDate = existingSale.InvoiceDate,
                    VchType = 5, // Sale VCH Type
                    Debit = existingSale.TotalAmount,
                    Credit = existingSale.TotalAmount,
                    SaleId = existingSale.Id,
                    UserId = userId,
                    RefNo = existingSale.InvoiceNumber,
                    Notes = "Updated Sales Entry",
                    Created = DateTime.Now,
                    SubscriptionId = subscriptionId
                };

                await _context.JournalPosts.AddAsync(journal);
                await _context.SaveChangesAsync();

                // Create new journal details
                var journalDetails = new List<JournalPostDetail>();
                if (existingSale.PaymentMethod == 0) // On Credit
                {
                    journalDetails.Add(new JournalPostDetail
                    {
                        ChartOfAccountId = 23, // Accounts Receivable
                        Debit = existingSale.TotalAmount,
                        Description = "Credit Sale - Receivable",
                        Remarks = "Accounts Receivable"
                    });
                }
                else if (existingSale.PaymentMethod == 1) // On Cash
                {
                    journalDetails.Add(new JournalPostDetail
                    {
                        ChartOfAccountId = 20, // Cash in Hand
                        Debit = existingSale.TotalAmount,
                        Description = "Cash Sale",
                        Remarks = "Cash received from sale"
                    });
                }

                // Sales Revenue Credit
                journalDetails.Add(new JournalPostDetail
                {
                    ChartOfAccountId = 24, // Sales Revenue
                    Credit = existingSale.TotalAmount,
                    Description = "Sales Revenue",
                    Remarks = "Credit revenue for sale"
                });

                // COGS and Inventory
                journalDetails.Add(new JournalPostDetail
                {
                    ChartOfAccountId = 25, // Cost of Goods Sold
                    Debit = existingSale.TotalAmount,
                    Description = "COGS Entry",
                    Remarks = "Record expense for COGS"
                });

                journalDetails.Add(new JournalPostDetail
                {
                    ChartOfAccountId = 22, // Inventory
                    Credit = existingSale.TotalAmount,
                    Description = "Inventory Reduction",
                    Remarks = "Reduce inventory for sold items"
                });

                // Finalize Journal Details
                foreach (var jd in journalDetails)
                {
                    jd.JournalPostId = journal.Id;
                    jd.BranchId = branchId;
                    jd.VchNo = journal.VchNo;
                    jd.VchDate = journal.VchDate;
                    jd.VchType = journal.VchType;
                    jd.BusinessYearId = journal.BusinessYearId;
                    jd.SaleId = existingSale.Id;
                    jd.SubscriptionId = subscriptionId;
                    jd.CreatedAt = DateTime.Now;
                }
                await _context.JournalPostDetails.AddRangeAsync(journalDetails);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"UpdateSale Error: {ex.Message}");
                throw new InvalidOperationException("An unexpected error occurred while updating the sale.", ex);
            }
        }

        public async Task<string> DeleteSale(int saleId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {                
                var sale = await _context.Sales
                    .Include(s => s.SaleDetails)
                    .FirstOrDefaultAsync(s => s.Id == saleId);

                if (sale == null)
                {
                    return "Sale not found.";
                }
                if (sale.PaymentStatus != 0)
                {
                    return "Cannot delete sale because it has a non-zero payment status.";
                }

                var subscriptionId = sale.SubscriptionId;
                var productStocks = await _context.ProductStocks
                    .Where(ps => ps.SourceType == "Sale" && ps.SourceId == saleId && ps.SubscriptionId == subscriptionId)
                    .ToListAsync();
                _context.ProductStocks.RemoveRange(productStocks);
                
                var journalPost = await _context.JournalPosts
                    .FirstOrDefaultAsync(jp => jp.SaleId == saleId && jp.SubscriptionId == subscriptionId);

                if (journalPost != null)
                {
                    var journalPostDetails = await _context.JournalPostDetails
                        .Where(jpd => jpd.JournalPostId == journalPost.Id && jpd.SubscriptionId == subscriptionId)
                        .ToListAsync();
                    _context.JournalPostDetails.RemoveRange(journalPostDetails);
                    _context.JournalPosts.Remove(journalPost);
                }
                
                if (sale.SaleDetails != null)
                {
                    _context.SaleDetails.RemoveRange(sale.SaleDetails);
                }
                
                _context.Sales.Remove(sale);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return "Sale deleted successfully.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"DeleteSale Error: {ex.Message}");
                return $"An error occurred while deleting the sale: {ex.Message}";
            }
        }

        public async Task<string> GenerateInvoiceNo()
        {
            var lastinvoice = await _context.Sales
                .OrderByDescending(s => s.Id)
                .FirstOrDefaultAsync();
            int nextNumber = 1;
            if (lastinvoice != null && !string.IsNullOrEmpty(lastinvoice.InvoiceNumber))
            {
                var parts = lastinvoice.InvoiceNumber.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }
            return $"SINV{nextNumber.ToString("D2")}";
        }

    }
}
