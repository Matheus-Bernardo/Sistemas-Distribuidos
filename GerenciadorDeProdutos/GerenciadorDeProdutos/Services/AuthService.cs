using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GerenciadorDeProdutos.DTOS;
using GerenciadorDeProdutos.Models;
using GerenciadorDeProdutos.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace GerenciadorDeProdutos.Services;

public class AuthService
{
    private readonly IMongoRepository<User> _repo;
    private readonly IConfiguration _config;
    public AuthService(IMongoRepository<User> repo, IConfiguration config)
    {
        _repo = repo;
        _config = config;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
    {
        var users = await _repo.GetAllAsync();
        var user = users.FirstOrDefault(x => x.Email == dto.Email);

        if (user is null) return null;

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        var (token, expiresAt) = GenerateJwt(user);

        return new LoginResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = new
            {
                id = user.Id,
                name = user.Name,
                email = user.Email,
                role = user.Role
            }
        };
    }


    public async Task<User> RegisterAsync(RegisterDto dto)
    {
        var users = await _repo.GetAllAsync();
        if (users.Any(x => x.Email == dto.Email))
            throw new Exception("Email já cadastrado.");

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Role = dto.Role,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        await _repo.CreateAsync(user);
        return user;
    }



    private (string token, DateTime expiresAt) GenerateJwt(User user)
    {
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

        var claims = new[]
        {
            new Claim("id", user.Id!),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role ?? "employee")
        };

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256
        );

        var expires = DateTime.UtcNow.AddHours(8);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return (jwt, expires);
    }

    
}