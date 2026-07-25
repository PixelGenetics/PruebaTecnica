using System.ComponentModel.DataAnnotations;

namespace PruebaTecnica.Application.Features.Auth.DTOs;

public class RegisterUserDto
{
    [Required]
    [MaxLength(50)]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Contrasenia { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Correo { get; set; } = string.Empty;
}