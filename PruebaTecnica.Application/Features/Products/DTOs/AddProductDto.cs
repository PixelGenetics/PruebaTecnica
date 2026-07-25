using System.ComponentModel.DataAnnotations;

namespace PruebaTecnica.Application.Features.Products.DTOs;

public class AddProductDto
{
    [Required]
    [MaxLength(50)]
    public required string Codigo { get; set; }

    [Required]
    [MaxLength(150)]
    public required string Nombre { get; set; }

    public int CategoryId { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Precio { get; set; }

    public bool Estado { get; set; } = true;
}