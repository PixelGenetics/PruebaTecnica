namespace PruebaTecnica.Application.Features.Products.DTOs;

public class ProductDto
{
    public int Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public string Categoria { get; set; } = string.Empty;

    public decimal Precio { get; set; }

    public bool Estado { get; set; }

    public DateTime FechaCreacion { get; set; }
}