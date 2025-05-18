using System.ComponentModel.DataAnnotations;

namespace AccuStock.Models.ViewModels.PurchaseReturn
{
    public class PurchaseReturnDetailVM
    {
        public int PurchaseDetailId { get; set; }
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Please specify the return quantity.")]
        [Range(1, int.MaxValue, ErrorMessage = "Return quantity must be greater than 0.")]
        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
        public decimal VatRate { get; set; }

        [StringLength(100)]
        public string? Reason { get; set; }
    }
}
