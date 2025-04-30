namespace AccuStock.Models
{
    public class ProductStock
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public DateTime Date { get; set; }
        public int QuantityIn { get; set; }
        public int QuantityOut { get; set; }
        public string? ReferenceNo { get; set; }
        public string? SourceType { get; set; } // e.g., "Purchase", "Sale", "Adjustment"
        public int SourceId { get; set; }
        public string? Remarks { get; set; }
    }
}
