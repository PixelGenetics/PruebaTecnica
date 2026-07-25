namespace PruebaTecnica.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductResult
{
    public bool Success { get; set; }

    public bool ProductNotFound { get; set; }

    public bool HasMovements { get; set; }
}