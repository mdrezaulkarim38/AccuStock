namespace AccuStock.Models.ViewModels.CustomerPayment
{
    public class CustomerPaymentRequestVM
    {
            public int SaleId { get; set; }
            public decimal AmountPaid { get; set; }
            public int? PaymentMethod { get; set; }
            public string? Notes { get; set; }        
    }
}
