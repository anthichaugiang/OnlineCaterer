using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using catere_be.Data;
using catere_be.Models;
using catere_be.Controllers;
using catere_be.Dto;

[Route("api/[controller]")]
[ApiController]
public class SupplierController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ImageController _imageController;

    public SupplierController(AppDbContext context,  ImageController imageController)
    {
        _context = context;
        _imageController = imageController;
    }
    [HttpGet("all")]
    public async Task<List<SupplierDto>> GetAllSuppliersWithServicesAndMinRoomPrice()
    {
        var suppliers = await _context.Supplier
            .Where(s => s.IsActive == false) 
            .Include(s => s.Services.Where(sv => sv.IsActive == false)) 
                .ThenInclude(sv => sv.Room.Where(r => r.IsActive == false)) 
            .Select(s => new
            {
                Supplier = s,
                MinRoomPrice = s.Services
                    .SelectMany(sv => sv.Room)
                    .Where(r => r.Price != null)
                    .DefaultIfEmpty()
                    .Min(r => r != null ? r.Price : (double?)0) ?? 0
            })
            .ToListAsync();

        // Chuyển đổi dữ liệu và thêm URL hình ảnh
        var supplierDtos = suppliers.Select(s => new SupplierDto
        {
            SupplierId = s.Supplier.SupplierId,
            Name = s.Supplier.Name,
            PhoneNumber = s.Supplier.PhoneNumber,
            Address = s.Supplier.Address,
            Email = s.Supplier.Email,
            Level = s.Supplier.Level,
            CityId = s.Supplier.CityId,
            ImageUrl = GetImageUrl(s.Supplier.ImageUrl),
            LoginName = s.Supplier.LoginName,
            IsActive = s.Supplier.IsActive,
            Services = s.Supplier.Services.Select(sv => new ServiceDto
            {
                ServiceId = sv.ServiceId,
                ServiceName = sv.ServiceName,
                Description = sv.Description,
                SupplierId = sv.SupplierId,
                ImageUrl = sv.ImageUrl,
                IsActive = sv.IsActive
            }).ToList(),
            MinRoomPrice = s.MinRoomPrice
        }).ToList();

        return supplierDtos;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<Supplier>>> GetSupplier()
    {
        var suppliers = await _context.Supplier.Where(c => !c.IsActive).OrderByDescending(c => c.SupplierId).ToListAsync();

        // Thêm URL hình ảnh vào mỗi đối tượng Supplier
        foreach (var Supplier in suppliers)
        {
            Supplier.ImageUrl = GetImageUrl(Supplier.ImageUrl);
        }

        return suppliers;
    }



    [HttpGet("{id}")]
    public async Task<ActionResult<Supplier>> GetSupplier(int id)
    {
        var supplier = await _context.Supplier.FindAsync(id);

        if (supplier == null)
        {
            return NotFound();
        }

        supplier.ImageUrl = GetImageUrl(supplier.ImageUrl);
        return supplier;
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

    [HttpPost]
    public async Task<ActionResult<Supplier>> PostSupplier([FromForm] Supplier Supplier, IFormFile? file)
    {

        bool exists = await _context.Supplier.AnyAsync(c => c.LoginName == Supplier.LoginName);

        if (exists)
        {
            return Conflict(new { message = " Login Name already exists." });
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
                    Supplier.ImageUrl = imagePath;
                }
            }

            // Hash mật khẩu của người dùng
            Supplier.PasswordHash = catere_be.Hash.PasswordHasher.HashPassword(Supplier.PasswordHash);

        // Thêm người dùng vào cơ sở dữ liệu
        var SupplierEntity = new Supplier
        {
            Name = Supplier.Name,
         
            
            PhoneNumber = Supplier.PhoneNumber,
            Email = Supplier.Email,
            Address = Supplier.Address,
            ImageUrl = Supplier.ImageUrl,
            CityId = Supplier.CityId,
            Level = Supplier.Level,
            LoginName = Supplier.LoginName,
            PasswordHash = Supplier.PasswordHash,
            IsActive = Supplier.IsActive
        };

        _context.Supplier.Add(SupplierEntity);
        await _context.SaveChangesAsync();

        Supplier.SupplierId = SupplierEntity.SupplierId;

        return CreatedAtAction("GetSupplier", new { id = Supplier.SupplierId }, Supplier);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutSupplier(int id, [FromForm] Supplier Supplier, IFormFile? file)
    {
        if (id != Supplier.SupplierId)
        {
            return BadRequest();
        }

        var existingSupplier = await _context.Supplier.FindAsync(id);
        if (existingSupplier == null)
        {
            return NotFound();
        }

        // Chỉ cập nhật các trường nếu có thay đổi
        existingSupplier.Name = Supplier.Name;
        existingSupplier.Email = Supplier.Email;
        existingSupplier.Address = Supplier.Address;
        existingSupplier.PhoneNumber = Supplier.PhoneNumber;
        existingSupplier.Email = Supplier.Email;
        existingSupplier.Address = Supplier.Address;
        existingSupplier.CityId = Supplier.CityId;
        existingSupplier.LoginName = Supplier.LoginName;

        // Nếu file không được gửi, giữ lại ImageUrl hiện tại
        if (file != null && file.Length > 0)
        {
            var uploadResult = await _imageController.UploadImage(file) as OkObjectResult;
            if (uploadResult != null)
            {
                var imagePath = uploadResult.Value.GetType().GetProperty("fileName").GetValue(uploadResult.Value, null).ToString();
                existingSupplier.ImageUrl = imagePath;
            }
        }

        // Nếu PasswordHash không được gửi, giữ lại PasswordHash hiện tại
        if (!string.IsNullOrEmpty(Supplier.PasswordHash))
        {
            existingSupplier.PasswordHash = catere_be.Hash.PasswordHasher.HashPassword(Supplier.PasswordHash);
        }

        _context.Entry(existingSupplier).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SupplierExists(id))
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
    public async Task<IActionResult> DeleteSupplier(int id)
    {
        var supplier = await _context.Supplier.FindAsync(id);
        if (supplier == null)
        {
            return NotFound();
        }

        supplier.IsActive = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool SupplierExists(int id)
    {
        return _context.Supplier.Any(e => e.SupplierId == id);
    }
}


