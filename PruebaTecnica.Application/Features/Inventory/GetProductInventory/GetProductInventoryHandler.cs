using MediatR;
using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Common.Interfaces;
using PruebaTecnica.Application.Features.Inventory.DTOs;

namespace PruebaTecnica.Application.Features.Inventory.Queries.GetProductInventory;

public class GetProductInventoryHandler
    : IRequestHandler<GetProductInventoryQuery, ProductInventoryDto?>
{
    private readonly IAppDbContext _context;

    public GetProductInventoryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<ProductInventoryDto?> Handle(
        GetProductInventoryQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _context.Product
            .AsNoTracking()
            .Include(product => product.Movimientos)
            .FirstOrDefaultAsync(
                product => product.Id == request.ProductId,
                cancellationToken);

        if (product is null)
        {
            return null;
        }

        var totalEntradas = product.Movimientos
            .Where(movement =>
                movement.Tipo.Equals(
                    "Entrada",
                    StringComparison.OrdinalIgnoreCase))
            .Sum(movement => movement.Cantidad);

        var totalSalidas = product.Movimientos
            .Where(movement =>
                movement.Tipo.Equals(
                    "Salida",
                    StringComparison.OrdinalIgnoreCase))
            .Sum(movement => movement.Cantidad);

        return new ProductInventoryDto
        {
            Id = product.Id,
            Codigo = product.Codigo,
            Nombre = product.Nombre,
            CategoryId = product.CategoryId,
            Precio = product.Precio,
            Estado = product.Estado,
            FechaCreacion = product.FechaCreacion,
            TotalEntradas = totalEntradas,
            TotalSalidas = totalSalidas,
            StockActual = totalEntradas - totalSalidas,

            Movimientos = product.Movimientos
                .OrderByDescending(movement => movement.Fecha)
                .Select(movement => new MovementDto
                {
                    Id = movement.Id,
                    Tipo = movement.Tipo.Equals(
                        "Entrada",
                        StringComparison.OrdinalIgnoreCase)
                            ? "Entrada"
                            : "Salida",
                    Cantidad = movement.Cantidad,
                    Fecha = movement.Fecha,
                    Referencia = movement.Referencia
                })
                .ToList()
        };
    }
}