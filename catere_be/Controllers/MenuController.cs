using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using catere_be.Data;
using catere_be.Models;
using catere_be.Controllers;
using System;

[Route("api/[controller]")]
[ApiController]
public class MenuController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ImageController _imageController;

    public MenuController(AppDbContext context, ImageController imageController)
    {
        _context = context;
        _imageController = imageController;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Menu>>> GetMenu()
    {
        var Menus = await _context.Menu.Where(c => !c.IsActive).OrderByDescending(c => c.MenuItemId).ToListAsync();
        foreach (var Menu in Menus)
        {
            Menu.ImageUrl = GetImageUrl(Menu.ImageUrl);
        }

        return Menus;
    }
    [HttpGet("supplier/{supplierId}")]
    public ActionResult<List<Menu>> GetMenusBySupplierId(int supplierId)
    {
        var menus = _context.Menu
            .Where(menu => menu.SupplierId == supplierId)
            .ToList();

        if (menus == null || !menus.Any())
        {
            return NotFound();
        }
        foreach (var Menu in menus)
        {
            Menu.ImageUrl = GetImageUrl(Menu.ImageUrl);
        }
        return Ok(menus);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<Menu>> GetMenu(int id)
    {
        var menu = await _context.Menu.FindAsync(id);

        if (menu == null)
        {
            return NotFound();
        }

        return menu;
    }

    [HttpPost]
    public async Task<ActionResult> PostMenu([FromForm] Menu Menu, IFormFile? file)
    {
        bool exists = await _context.Menu.AnyAsync(c => c.MenuItemId == Menu.MenuItemId);

        if (exists)
        {
            return Conflict(new { message = " this already exists." });
        }
        try
        {
            if (file != null && file.Length > 0)
            {
                // Upload image and get image path
                var uploadResult = await _imageController.UploadImage(file) as OkObjectResult;
                if (uploadResult != null)
                {
                    var imagePath = uploadResult.Value.GetType().GetProperty("fileName").GetValue(uploadResult.Value, null).ToString();
                    Menu.ImageUrl = imagePath;
                }
            }

           
            // Thêm người dùng vào cơ sở dữ liệu
            var MenuEntity = new Menu
            {
                ItemName = Menu.ItemName,
                Price = Menu.Price,
                ImageUrl = Menu.ImageUrl,
                SupplierId = Menu.SupplierId,
                CategoryId = Menu.CategoryId,
                IsActive = false
            };

            _context.Menu.Add(MenuEntity);
            await _context.SaveChangesAsync();

            Menu.MenuItemId = MenuEntity.MenuItemId;

            return CreatedAtAction("GetMenu", new { id = Menu.MenuItemId }, Menu);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutMenu(int id, [FromForm] Menu Menu, IFormFile? file)
    {
        if (id != Menu.MenuItemId)
        {
            return BadRequest();
        }

        var existingMenu = await _context.Menu.FindAsync(id);
        if (existingMenu == null)
        {
            return NotFound();
        }

        // Chỉ cập nhật các trường nếu có thay đổi
        existingMenu.ItemName = Menu.ItemName;
        existingMenu.SupplierId = Menu.SupplierId;
        existingMenu.CategoryId= Menu.CategoryId;
       

        // Nếu file không được gửi, giữ lại ImageUrl hiện tại
        if (file != null && file.Length > 0)
        {
            var uploadResult = await _imageController.UploadImage(file) as OkObjectResult;
            if (uploadResult != null)
            {
                var imagePath = uploadResult.Value.GetType().GetProperty("fileName").GetValue(uploadResult.Value, null).ToString();
                existingMenu.ImageUrl = imagePath;
            }
        }


        _context.Entry(existingMenu).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MenuExists(id))
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
    public async Task<IActionResult> DeleteMenu(int id)
    {
        var menu = await _context.Menu.FindAsync(id);
        if (menu == null)
        {
            return NotFound();
        }

        _context.Menu.Remove(menu);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool MenuExists(int id)
    {
        return _context.Menu.Any(e => e.MenuItemId == id);
    }
    private string GetImageUrl(string imageName)
    {
        // Kiểm tra nếu imageName là null hoặc rỗng
        if (string.IsNullOrEmpty(imageName))
        {
            return string.Empty;
        }

        // Xây dựng URL dẫn đến hình ảnh từ ImageController
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        return $"{baseUrl}/api/Image/{imageName}";
    }
}
