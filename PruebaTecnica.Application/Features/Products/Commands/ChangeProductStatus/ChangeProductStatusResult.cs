namespace PruebaTecnica.Application.Features.Products.Commands.ChangeProductStatus;

public class ChangeProductStatusResult
{
    public bool Success { get; set; }

    public bool ProductNotFound { get; set; }

    public string? ErrorMessage { get; set; }

    public string? Message { get; set; }

    public int Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public decimal Precio { get; set; }

    public bool Estado { get; set; }
}