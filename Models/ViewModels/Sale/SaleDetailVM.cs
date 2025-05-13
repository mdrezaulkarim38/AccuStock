namespace AccuStock.Models.ViewModels.Sale
{
    public class SaleDetailVM
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal VatRate { get; set; }
    }
}
