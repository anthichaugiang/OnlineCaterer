using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using catere_be.Data;
using catere_be.Models;
using catere_be.Controllers;
using catere_be.Dto;

[Route("api/[controller]")]
[ApiController]
public class ServiceController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ImageController _imageController;

    public ServiceController(AppDbContext context, ImageController imageController)
    {
        _context = context;
        _imageController = imageController;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Service>>> GetService()
    {
        var services = await _context.Service.Where(c => !c.IsActive).OrderByDescending(c => c.ServiceId).ToListAsync();

        foreach (var service in services)
        {
            service.ImageUrl = GetImageUrl(service.ImageUrl);
        }

        return services;
    }

    [HttpGet("supplier/{supplierId}")]
    public async Task<ActionResult<List<ServiceDto>>> GetServicesBySupplierId(int supplierId)
    {
        // Fetch the services from the database first
        var services = await _context.Service
            .Include(service => service.Room)
            .Where(service => service.SupplierId == supplierId)
            .ToListAsync();

        if (services == null || !services.Any())
        {
            return NotFound();
        }

        // Create the ServiceDto objects and set the ImageUrl
        var serviceDtos = services.Select(service => new ServiceDto
        {
            ServiceId = service.ServiceId,
            ServiceName = service.ServiceName,
            Description = service.Description,
            SupplierId = service.SupplierId,
            ImageUrl = GetImageUrl(service.ImageUrl),
            IsActive = service.IsActive,
            Rooms = service.Room.Select(room => new RoomDto
            {
                RoomId = room.RoomId,
                RoomName = room.RoomName,
                Capacity = room.Capacity,
                Price = room.Price,
                ServiceId = room.ServiceId,
                Description = room.Description,
                IsActive = room.IsActive
            }).ToList()
        }).ToList();

        return Ok(serviceDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Service>> GetService(int id)
    {
        var service = await _context.Service.FindAsync(id);

        if (service == null)
        {
            return NotFound();
        }

        service.ImageUrl = GetImageUrl(service.ImageUrl);

        return service;
    }

    private string GetImageUrl(string imageName)
    {
        if (string.IsNullOrEmpty(imageName))
        {
            return string.Empty;
        }

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        return $"{baseUrl}/api/Image/{imageName}";
    }

    [HttpPost]
    public async Task<ActionResult> PostService([FromForm] Service service, IFormFile? file)
    {
        bool exists = await _context.Service.AnyAsync(c => c.ServiceId == service.ServiceId);

        if (exists)
        {
            return Conflict(new { message = "Service already exists." });
        }

        try
        {
            if (file != null && file.Length > 0)
            {
                var uploadResult = await _imageController.UploadImage(file) as OkObjectResult;
                if (uploadResult != null)
                {
                    var imagePath = uploadResult.Value.GetType().GetProperty("fileName").GetValue(uploadResult.Value, null).ToString();
                    service.ImageUrl = imagePath;
                }
            }

            var serviceEntity = new Service
            {
                ServiceName = service.ServiceName,
                Description = service.Description,
                SupplierId = service.SupplierId,
                ImageUrl = service.ImageUrl,
            };

            _context.Service.Add(serviceEntity);
            await _context.SaveChangesAsync();

            service.ServiceId = serviceEntity.ServiceId;

            return CreatedAtAction("GetService", new { id = service.ServiceId }, service);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutService(int id, [FromForm] Service service, IFormFile? file)
    {
        if (id != service.ServiceId)
        {
            return BadRequest();
        }

        var existingService = await _context.Service.FindAsync(id);
        if (existingService == null)
        {
            return NotFound();
        }

        existingService.ServiceName = service.ServiceName;
        existingService.SupplierId = service.SupplierId;
        existingService.Description = service.Description;

        if (file != null && file.Length > 0)
        {
            var uploadResult = await _imageController.UploadImage(file) as OkObjectResult;
            if (uploadResult != null)
            {
                var imagePath = uploadResult.Value.GetType().GetProperty("fileName").GetValue(uploadResult.Value, null).ToString();
                existingService.ImageUrl = imagePath;
            }
        }

        _context.Entry(existingService).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ServiceExists(id))
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
    public async Task<IActionResult> DeleteService(int id)
    {
        var service = await _context.Service.FindAsync(id);
        if (service == null)
        {
            return NotFound();
        }

        _context.Service.Remove(service);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ServiceExists(int id)
    {
        return _context.Service.Any(e => e.ServiceId == id);
    }
}
