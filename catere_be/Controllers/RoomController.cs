using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using catere_be.Data;
using catere_be.Models;
using catere_be.Dto;

[Route("api/[controller]")]
[ApiController]
public class RoomController : ControllerBase
{
    private readonly AppDbContext _context;

    public RoomController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Room>>> GetRoom()
    {
        return await _context.Room.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Room>> GetRoom(int id)
    {
        var room = await _context.Room.FindAsync(id);

        if (room == null)
        {
            return NotFound();
        }

        return room;
    }

    [HttpPost]
    public async Task<ActionResult<Room>> PostRoom(Room room)
    {
        _context.Room.Add(room);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetRoom", new { id = room.RoomId }, room);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRoom(int id, [FromBody] RoomDto roomDto)
    {
        if (id != roomDto.RoomId)
        {
            return BadRequest();
        }

        var existingRoom = await _context.Room.FindAsync(id);
        if (existingRoom == null)
        {
            return NotFound();
        }

        // Cập nhật các trường cần thiết
        existingRoom.RoomName = roomDto.RoomName;
        existingRoom.Capacity = roomDto.Capacity;
        existingRoom.Price = roomDto.Price;
        existingRoom.ServiceId = roomDto.ServiceId;
        existingRoom.Description = roomDto.Description;

        _context.Entry(existingRoom).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!RoomExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        var room = await _context.Room.FindAsync(id);
        if (room == null)
        {
            return NotFound();
        }

        _context.Room.Remove(room);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool RoomExists(int id)
    {
        return _context.Room.Any(e => e.RoomId == id);
    }
}
