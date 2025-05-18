namespace AccuStock.Models.ViewModels.PurchaseReturn
{
    public class PurchaseReturnListViewModel
    {
        public int Id { get; set; }
        public string ReturnNo { get; set; }
        public string PurchaseNo { get; set; }
        public string VendorName { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }
}
