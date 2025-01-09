using Microsoft.AspNetCore.Mvc;
using catere_be.Data;
using catere_be.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;

namespace catere_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ImageController _imageController;

        public CustomerController(AppDbContext context, ImageController imageController)
        {
            _context = context;
            _imageController = imageController;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            var customers = await _context.Customer.Where(c => !c.IsActive).OrderByDescending(c => c.CustomerId).ToListAsync();

            // Thêm URL hình ảnh vào mỗi đối tượng Customer
            foreach (var customer in customers)
            {
                customer.ImageUrl = GetImageUrl(customer.ImageUrl);
            }

            return customers;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customer.FindAsync(id);

            if (customer == null || customer.IsActive)
            {
                return NotFound();
            }

            // Lấy URL hình ảnh
            customer.ImageUrl = GetImageUrl(customer.ImageUrl);

            return customer;
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
        public async Task<ActionResult> PostCustomer([FromForm] Customer customer, IFormFile ?file)
        {
            bool exists = await _context.Customer.AnyAsync(c =>  c.LoginName == customer.LoginName);

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
                        customer.ImageUrl = imagePath;
                    }
                }

                // Hash mật khẩu của người dùng
                customer.PasswordHash = Hash.PasswordHasher.HashPassword(customer.PasswordHash);

                // Thêm người dùng vào cơ sở dữ liệu
                var customerEntity = new Customer
                {
                    FirstName = customer.FirstName,
                    MiddleName = customer.MiddleName,
                    LastName = customer.LastName,
                    Gender = customer.Gender,
                    DateOfBirth = customer.DateOfBirth,
                    PhoneNumber = customer.PhoneNumber,
                    Email = customer.Email,
                    Address = customer.Address,
                    ImageUrl = customer.ImageUrl,
                    CustomerType = customer.CustomerType,
                    LoginName = customer.LoginName,
                    PasswordHash = customer.PasswordHash,
                    IsActive = customer.IsActive
                };

                _context.Customer.Add(customerEntity);
                await _context.SaveChangesAsync();

                customer.CustomerId = customerEntity.CustomerId;

                return CreatedAtAction("GetCustomer", new { id = customer.CustomerId }, customer);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, [FromForm] Customer customer,  IFormFile ?file)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            var existingCustomer = await _context.Customer.FindAsync(id);
            if (existingCustomer == null)
            {
                return NotFound();
            }

            // Chỉ cập nhật các trường nếu có thay đổi
            existingCustomer.FirstName = customer.FirstName;
            existingCustomer.MiddleName = customer.MiddleName;
            existingCustomer.LastName = customer.LastName;
            existingCustomer.Gender = customer.Gender != existingCustomer.Gender ? customer.Gender : existingCustomer.Gender;
            existingCustomer.DateOfBirth = customer.DateOfBirth != default ? customer.DateOfBirth : existingCustomer.DateOfBirth;
            existingCustomer.PhoneNumber = customer.PhoneNumber;
            existingCustomer.Email = customer.Email;
            existingCustomer.Address = customer.Address;
            existingCustomer.CustomerType = customer.CustomerType;
            existingCustomer.LoginName = customer.LoginName;

            // Nếu file không được gửi, giữ lại ImageUrl hiện tại
            if (file != null && file.Length > 0)
            {
                var uploadResult = await _imageController.UploadImage(file) as OkObjectResult;
                if (uploadResult != null)
                {
                    var imagePath = uploadResult.Value.GetType().GetProperty("fileName").GetValue(uploadResult.Value, null).ToString();
                    existingCustomer.ImageUrl = imagePath;
                }
            }

            // Nếu PasswordHash không được gửi, giữ lại PasswordHash hiện tại
            if (!string.IsNullOrEmpty(customer.PasswordHash))
            {
                existingCustomer.PasswordHash = Hash.PasswordHasher.HashPassword(customer.PasswordHash);
            }

            _context.Entry(existingCustomer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customer.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            // Thay đổi trạng thái của khách hàng thành inactive
            customer.IsActive = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return _context.Customer.Any(e => e.CustomerId == id);
        }
    }
}
