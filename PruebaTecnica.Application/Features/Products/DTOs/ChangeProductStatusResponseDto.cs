namespace PruebaTecnica.Application.Features.Products.DTOs;

public sealed class ChangeProductStatusResponseDto
{
    public string? Mensaje { get; set; } = string.Empty;
    public ProductStatusDto Producto { get; set; } = null!;

}

public sealed class ProductStatusDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public bool Estado { get; set; }
}

