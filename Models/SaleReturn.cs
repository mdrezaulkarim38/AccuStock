namespace AccuStock.Models
{
    public class SaleReturn
    {
        public int Id { get; set; }
        public int SaleDetailId { get; set; }
        public SaleDetails? SaleDetail { get; set; }
        public int ReturnQuantity { get; set; }
        public string? Reason { get; set; }
        public DateTime ReturnDate { get; set; } = DateTime.Now;
        public decimal RefundAmount { get; set; }
        public bool Status { get; set; } = true;
    }
}
