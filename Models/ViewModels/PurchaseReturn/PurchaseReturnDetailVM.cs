using System.ComponentModel.DataAnnotations;

namespace AccuStock.Models.ViewModels.PurchaseReturn
{
    public class PurchaseReturnDetailVM
    {
        public int PurchaseDetailId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal VatRate { get; set; }
        [StringLength(100)]
        public string? Reason { get; set; }
    }
}
