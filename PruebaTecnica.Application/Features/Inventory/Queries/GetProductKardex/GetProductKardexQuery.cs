using MediatR;
using PruebaTecnica.Application.Features.Inventory.DTOs;

namespace PruebaTecnica.Application.Features.Inventory.Queries.GetProductKardex;

public record GetProductKardexQuery(int ProductId)
    : IRequest<ProductKardexDto?>;