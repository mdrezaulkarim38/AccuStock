namespace AccuStock.Models.ViewModels.PurchaseReturn
{
    public class PurchaseDetailVM
    {
        public int Id { get; set; } // PurchaseDetail.Id
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; } // Purchased quantity
        public int ReturnedQuantity { get; set; } // Already returned
        public int AvailableToReturn { get; set; } // Quantity - ReturnedQuantity
        public decimal UnitPrice { get; set; }
        public decimal VatRate { get; set; }
    }
}
