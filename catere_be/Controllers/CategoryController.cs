using Microsoft.AspNetCore.Mvc;
using catere_be.Data;
using catere_be.Models;
using Microsoft.EntityFrameworkCore;


[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoryController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategory()
    {
        return await _context.Category.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetCategory(int id)
    {
        var Category = await _context.Category.FindAsync(id);

        if (Category == null)
        {
            return NotFound();
        }

        return Category;
    }

    [HttpPost]
    public async Task<ActionResult<Category>> PostCategory(Category Category)
    {
        _context.Category.Add(Category);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetCategory", new { id = Category.CategoryId }, Category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCategory(int id, Category Category)
    {
        if (id != Category.CategoryId)
        {
            return BadRequest();
        }

        _context.Entry(Category).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CategoryExists(id))
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
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var Category = await _context.Category.FindAsync(id);
        if (Category == null)
        {
            return NotFound();
        }

        _context.Category.Remove(Category);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CategoryExists(int id)
    {
        return _context.Category.Any(e => e.CategoryId == id);
    }
}
