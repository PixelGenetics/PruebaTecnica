using MediatR;
using PruebaTecnica.Application.Features.Products.DTOs;

namespace PruebaTecnica.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Codigo,
    string Nombre,
    int CategoryId,
    decimal Precio,
    bool Estado
) : IRequest<ProductDto>;