namespace AccuStock.Models
{
    public class ReportRequest
    {
        public string ReportType { get; set; }
        public string TimeToSend { get; set; } // Format: "HH:mm"
        public string UserEmail { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int SubscriptionId { get; set; }
    }
}
