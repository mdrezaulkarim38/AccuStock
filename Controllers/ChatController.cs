using AccuStock.Data;
using AccuStock.Models;
using AccuStock.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccuStock.Controllers;
[Authorize]
public class ChatController : Controller
{
    private readonly BaseService _service;
    private readonly AppDbContext _context;

    public ChatController(BaseService service, AppDbContext context)
    {
        _service = service;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var currentUserId = _service.GetUserId().ToString();
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Unauthorized();
        }

        var users = await _context.Users
            .Where(u => u.Id.ToString() != currentUserId && u.SubscriptionId == _service.GetSubscriptionId())
            .Select(u => new User
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email
            })
            .ToListAsync();

        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> GetMessages(string receiverId)
    {
        var senderId = _service.GetUserId().ToString();
        var messages = await _context.ChatMessages
            .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                       (m.SenderId == receiverId && m.ReceiverId == senderId))
            .OrderBy(m => m.SentAt)
            .ToListAsync();
        return Json(messages);
    }
}