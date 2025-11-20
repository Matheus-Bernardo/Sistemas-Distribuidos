namespace GerenciadorDeProdutos.DTOS;

public class LoginResponseDto
{
    public string Token { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public object User { get; set; } = default!;
}