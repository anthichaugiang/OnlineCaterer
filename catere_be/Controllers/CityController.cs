using Microsoft.AspNetCore.Mvc;
using catere_be.Data;
using catere_be.Models;
using Microsoft.EntityFrameworkCore;


[Route("api/[controller]")]
[ApiController]
public class CityController : ControllerBase
{
    private readonly AppDbContext _context;

    public CityController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<City>>> GetCity()
    {
        return await _context.City.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<City>> GetCity(int id)
    {
        var city = await _context.City.FindAsync(id);

        if (city == null)
        {
            return NotFound();
        }

        return city;
    }

    [HttpPost]
    public async Task<ActionResult<City>> PostCity(City city)
    {
        _context.City.Add(city);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetCity", new { id = city.CityId }, city);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCity(int id, City city)
    {
        if (id != city.CityId)
        {
            return BadRequest();
        }

        _context.Entry(city).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CityExists(id))
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
    public async Task<IActionResult> DeleteCity(int id)
    {
        var city = await _context.City.FindAsync(id);
        if (city == null)
        {
            return NotFound();
        }

        _context.City.Remove(city);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CityExists(int id)
    {
        return _context.City.Any(e => e.CityId == id);
    }
}
