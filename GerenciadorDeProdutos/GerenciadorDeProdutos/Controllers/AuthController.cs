using GerenciadorDeProdutos.DTOS;
using GerenciadorDeProdutos.Services;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorDeProdutos.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController:ControllerBase
{
    private readonly AuthService _auth;

    public AuthController(AuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = await _auth.RegisterAsync(dto);
        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var response = await _auth.LoginAsync(dto);
        if (response is null) return Unauthorized("Credenciais inválidas");

        return Ok(response);
    }

}