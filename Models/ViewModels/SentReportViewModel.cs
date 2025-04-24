namespace AccuStock.Models.ViewModels;
public class SentReportViewModel
{
    public string ReportType { get; set; }
    public string TimeToSend { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
