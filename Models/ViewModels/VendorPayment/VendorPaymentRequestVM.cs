namespace AccuStock.Models.ViewModels.VendorPayment
{
    public class VendorPaymentRequestVM
    {
        public int PurchaseId { get; set; }
        public decimal AmountPaid { get; set; }
        public int? PaymentMethod { get; set; }
        public string? Notes { get; set; }
    }
}
