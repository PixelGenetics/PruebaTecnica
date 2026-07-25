using MediatR;
using PruebaTecnica.Application.Features.Products.DTOs;

namespace PruebaTecnica.Application.Features.Products.Queries.GetAllProducts;

public record GetAllProductsQuery(
    string? Nombre,
    int? CategoryId,
    bool? Estado,
    int Pagina,
    int CantidadPorPagina,
    string OrdenarPor,
    string Direccion
) : IRequest<PagedProductsDto>;