using MediatR;
using PruebaTecnica.Application.Features.Products.DTOs;

namespace PruebaTecnica.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    int Id,
    string Codigo,
    string Nombre,
    decimal Precio,
    bool Estado
) : IRequest<ProductDto?>;