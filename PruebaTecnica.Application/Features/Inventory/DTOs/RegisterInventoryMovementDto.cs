using System.ComponentModel.DataAnnotations;

namespace PruebaTecnica.Application.Features.Inventory.DTOs;

public class RegisterInventoryMovementDto
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public string Tipo { get; set; } = string.Empty;

    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "La cantidad debe ser mayor que cero.")]
    public int Cantidad { get; set; }

    [Required]
    [MaxLength(200)]
    public string Referencia { get; set; } = string.Empty;
}