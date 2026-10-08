
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using HealthcarePortalApi.Data;
using HealthcarePortalApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;


namespace HealthcarePortalApi.Services;

public class AuthService(ApplicationDbContext _context, IConfiguration _config)
{

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        // Prevent duplicate emails
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return null; 

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password), 
            Role = Role.doctor
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new AuthResponseDto(GenerateJwtToken(user), user.Email, user.Role.ToString()!);
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == dto.Email);
        
        // Verify user exists and password matches the hash
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null; 

        return new AuthResponseDto(GenerateJwtToken(user), user.Email, user.Role.ToString()!);
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2), 
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}