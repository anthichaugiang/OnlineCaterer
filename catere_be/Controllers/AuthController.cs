using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using catere_be.Data;
using catere_be.Dto;
using catere_be.Hash;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel loginModel)
    {
        var customer = await _context.Customer.SingleOrDefaultAsync(c => c.LoginName == loginModel.Username);
        var supplier = await _context.Supplier.SingleOrDefaultAsync(s => s.LoginName == loginModel.Username);

        if (customer == null && supplier == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        bool isPasswordValid = false;
        string userId = "";
        string userType = "";
        string idClaim = "";

        if (customer != null && PasswordHasher.VerifyPassword(customer.PasswordHash, loginModel.Password))
        {
            isPasswordValid = true;
            userId = customer.CustomerId.ToString();
            userType = "Customer";
            idClaim = customer.CustomerId.ToString();
        }
        else if (supplier != null && PasswordHasher.VerifyPassword(supplier.PasswordHash, loginModel.Password))
        {
            isPasswordValid = true;
            userId = supplier.SupplierId.ToString();
            userType = "Supplier";
            idClaim = supplier.SupplierId.ToString();
        }

        if (!isPasswordValid)
        {
            return Unauthorized("Invalid username or password.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, userType),
                new Claim("UserId", idClaim) 
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Ok(new AuthResponse
        {
            Token = tokenHandler.WriteToken(token),
            UserId = idClaim,
            UserType = userType
        });
    }
}
