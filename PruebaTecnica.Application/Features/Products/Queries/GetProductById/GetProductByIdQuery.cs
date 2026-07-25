using MediatR;
using PruebaTecnica.Application.Features.Products.DTOs;

namespace PruebaTecnica.Application.Features.Products.Queries.GetProductById;

public record GetProductByIdQuery(int Id) : IRequest<ProductDto?>;