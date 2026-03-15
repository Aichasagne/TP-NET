using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BooksApi.Models;
using BooksApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BooksApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly IConfiguration _configuration;

    public AuthController(AuthService authService, IConfiguration configuration)
    {
        _authService = authService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        var existingUser = await _authService.GetByUsernameAsync(model.Username);
        if (existingUser != null)
            return BadRequest("User already exists!");

        var user = new User
        {
            Username = model.Username,
            Email = model.Email,
            PasswordHash = _authService.HashPassword(model.Password),
            Role = "User"
        };

        await _authService.CreateAsync(user);
        return Ok("User created successfully!");
    }

    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdmin(RegisterModel model)
    {
        var existingUser = await _authService.GetByUsernameAsync(model.Username);
        if (existingUser != null)
            return BadRequest("User already exists!");

        var user = new User
        {
            Username = model.Username,
            Email = model.Email,
            PasswordHash = _authService.HashPassword(model.Password),
            Role = "Admin"
        };

        await _authService.CreateAsync(user);
        return Ok("Admin created successfully!");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        var user = await _authService.GetByUsernameAsync(model.Username);
        if (user == null || !_authService.VerifyPassword(model.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials!");

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = GenerateToken(authClaims);

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo,
            role = user.Role
        });
    }

    private JwtSecurityToken GenerateToken(List<Claim> claims)
    {
        var secret = _configuration["JwtSettings:Secret"]!;
        var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        return new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            expires: DateTime.UtcNow.AddDays(7),
            claims: claims,
            signingCredentials: new SigningCredentials(
                authKey, SecurityAlgorithms.HmacSha256)
        );
    }
}