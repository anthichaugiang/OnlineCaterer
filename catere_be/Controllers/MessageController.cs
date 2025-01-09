using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using catere_be.Dto; 
using catere_be.Data;
using catere_be.Models;
namespace catere_be.Controllers { 

[Route("api/[controller]")]
[ApiController]
public class MessageController : ControllerBase
{
    private readonly AppDbContext _context;

    public MessageController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<Message>> SendMessage([FromBody] Message message)
    {
        message.SentDate = DateTime.UtcNow;
        message.IsActive = true;

        _context.Message.Add(message);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMessage), new { id = message.MessageId }, message);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Message>> GetMessage(int id)
    {
        var message = await _context.Message.FindAsync(id);

        if (message == null)
        {
            return NotFound();
        }

        return message;
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult> GetMessagesByCustomerId(int customerId)
    {
        var groupedMessages = await _context.Message
            .Where(m => m.CustomerId == customerId)
            .OrderByDescending(m => m.SentDate)
            .GroupBy(m => m.SupplierId)
            .Select(g => new
            {
                SupplierId = g.Key,
                Messages = g.ToList()
            })
            .ToListAsync();

        return Ok(groupedMessages);
    }


        [HttpGet("supplier/{supplierId}")]
        public async Task<ActionResult> GetMessagesBySupplierId(int supplierId)
        {
            var groupedMessages = await _context.Message
                .Where(m => m.SupplierId == supplierId)
                .OrderByDescending(m => m.SentDate)
                .GroupBy(m => m.CustomerId)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    Messages = g.ToList()
                })
                .ToListAsync();

            return Ok(groupedMessages);
        }

    }
}