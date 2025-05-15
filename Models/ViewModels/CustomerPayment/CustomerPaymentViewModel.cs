namespace AccuStock.Models.ViewModels.CustomerPayment
{
    public class CustomerPaymentViewModel
    {
        public int SaleId { get; set; }
        public string? SaleNo { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal BillAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountDue => BillAmount - AmountPaid;
    }
}
