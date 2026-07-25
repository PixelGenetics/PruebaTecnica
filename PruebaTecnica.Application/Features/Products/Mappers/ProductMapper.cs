using PruebaTecnica.Application.Features.Products.DTOs;
using PruebaTecnica.Domain.Entities;

namespace PruebaTecnica.Application.Features.Products.Mappers;

public static class ProductMapper
{
    public static ProductDto ToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Codigo = product.Codigo,
            Nombre = product.Nombre,
            CategoryId = product.CategoryId,
            Categoria = product.Category?.Nombre ?? string.Empty,
            Precio = product.Precio,
            Estado = product.Estado,
            FechaCreacion = product.FechaCreacion
        };
    }
}