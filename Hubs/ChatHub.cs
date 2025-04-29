using AccuStock.Data;
using AccuStock.Models;
using AccuStock.Services;
using Microsoft.AspNetCore.SignalR;
namespace AccuStock.Hubs;
public class ChatHub : Hub
{
    private readonly BaseService _service;
    private readonly AppDbContext _context;

    public ChatHub(BaseService service, AppDbContext context)
    {
        _service = service;
        _context = context;
    }

    public async Task SendMessage(string receiverId, string message)
    {
        if (string.IsNullOrWhiteSpace(receiverId) || string.IsNullOrWhiteSpace(message))
        {
            throw new HubException("Receiver ID and message cannot be empty.");
        }

        var senderId = _service.GetUserId().ToString();
        if (string.IsNullOrEmpty(senderId))
        {
            throw new HubException("Sender not authenticated.");
        }

        var chatMessage = new ChatMessage
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Message = message.Trim(),
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        _context.ChatMessages.Add(chatMessage);
        await _context.SaveChangesAsync();

        await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, message);
        await Clients.Caller.SendAsync("ReceiveMessage", senderId, message);
    }

    public override async Task OnConnectedAsync()
    {
        var userId = _service.GetUserId().ToString();
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = _service.GetUserId().ToString();
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
        }
        await base.OnDisconnectedAsync(exception);
    }
}