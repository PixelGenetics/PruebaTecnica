using MediatR;
using PruebaTecnica.Application.Features.Inventory.DTOs;

namespace PruebaTecnica.Application.Features.Inventory.Queries.GetProductInventory;

public record GetProductInventoryQuery(int ProductId)
    : IRequest<ProductInventoryDto?>;