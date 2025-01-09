using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace catere_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly string _uploadFolder = "uploads";

        [HttpPost("image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File not selected");

            // Tạo đường dẫn lưu trữ
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), _uploadFolder);
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // Tạo tên file mới để tránh trùng lặp
            var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            // Lưu file vào thư mục
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Trả về tên file đã lưu để lưu vào cơ sở dữ liệu
            return Ok(new { fileName });
        }

        [HttpGet("{imageName}")]
        public IActionResult GetImage(string imageName)
        {
            // Tạo đường dẫn đến ảnh
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), _uploadFolder, imageName);

            // Kiểm tra xem file có tồn tại không
            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound("Image not found");
            }

            // Đọc dữ liệu từ file
            var imageBytes = System.IO.File.ReadAllBytes(imagePath);

            return new FileContentResult(imageBytes, "application/octet-stream");
        }
    }
}
