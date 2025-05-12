namespace AccuStock.Models.ViewModels.VendorPayment
{
    public class VendorPaymentViewModel
    {
        public int PurchaseId { get; set; }
        public string? PurchaseNo { get; set; }
        public int VendorId { get; set; }
        public string? VendorName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal BillAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountDue => BillAmount - AmountPaid;
    }
}
