using AccuStock.Data;
using AccuStock.Models;
using AccuStock.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccuStock.Controllers;
public class ChatController : Controller
{
    private readonly BaseService _service;
    private readonly AppDbContext _context;

    public ChatController(BaseService service, AppDbContext context)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IActionResult> Index()
    {
        var currentUserId = _service.GetUserId().ToString();
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Unauthorized();
        }

        var users = await _context.Users
            .Where(u => u.Id.ToString() != currentUserId)
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