using System.ComponentModel.DataAnnotations;

namespace AccuStock.Models;
public class ChatMessage
{
    public int Id { get; set; }
    [StringLength(100)]
    public string? SenderId { get; set; } // Identity User ID
    [StringLength(100)]
    public string? ReceiverId { get; set; } // Identity User ID
    [StringLength(250)]
    public string? Message { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;
}