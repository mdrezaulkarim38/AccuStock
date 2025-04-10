namespace AccuStock.Models;
public class ChatMessage
{
    public int Id { get; set; }
    public string? SenderId { get; set; } // Identity User ID
    public string? ReceiverId { get; set; } // Identity User ID
    public string? Message { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;
}