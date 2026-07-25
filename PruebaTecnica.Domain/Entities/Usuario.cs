using System.ComponentModel.DataAnnotations;

namespace PruebaTecnica.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Contrasenia { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    [EmailAddress]
    public string Correo { get; set; } = string.Empty;

    public bool Estado { get; set; } = true;
}