using System.ComponentModel.DataAnnotations;

namespace PruebaTecnica.Application.Features.Auth.DTOs;

public class LoginDto
{
    [Required]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required]
    public string Contrasenia { get; set; } = string.Empty;
}