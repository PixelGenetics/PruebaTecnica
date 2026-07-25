namespace PruebaTecnica.Application.Features.Auth.DTOs;

public class LoginResponseDto
{
    public int UsuarioId { get; set; }

    public string NombreUsuario { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;

    public DateTime Expiracion { get; set; }
}