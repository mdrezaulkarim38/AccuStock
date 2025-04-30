using System.ComponentModel.DataAnnotations;

namespace AccuStock.Models.ViewModels.Purchase
{
    public class PurchaseDetailViewModel
    {
        [Required]        
        public int ProductId { get; set; }
        [Required]
        public decimal Quantity { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }
        [Required]        
        public decimal VatRate { get; set; }
    }
}
