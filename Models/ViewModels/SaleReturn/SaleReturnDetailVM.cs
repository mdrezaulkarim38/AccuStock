using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AccuStock.Models.ViewModels.SaleReturn
{
    public class SaleReturnDetailVM
    {
        public int SaleDetailId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal VatRate { get; set; }
        [StringLength(100)]
        public string? Reason { get; set; }
    }
    public class SaleReturnListViewModel
    {
        public int Id { get; set; }
        public string? ReturnNo { get; set; }
        public string? SaleNo { get; set; }
        public string? CustomerName { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }
    }
    public class SaleReturnVM
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public int BranchId { get; set; }
        [DataType(DataType.Date)]
        public DateTime ReturnDate { get; set; }
        [StringLength(100)]
        public string? Notes { get; set; }
        public SelectList? SaleList { get; set; }
        public SelectList? BranchList { get; set; }
        public List<SaleReturnDetailVM> Details { get; set; } = new List<SaleReturnDetailVM>();
    }
}
