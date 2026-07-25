using MediatR;
using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Common.Interfaces;
using PruebaTecnica.Application.Features.Inventory.DTOs;

namespace PruebaTecnica.Application.Features.Inventory.Queries.GetProductKardex;

public class GetProductKardexHandler
    : IRequestHandler<GetProductKardexQuery, ProductKardexDto?>
{
    private readonly IAppDbContext _context;

    public GetProductKardexHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<ProductKardexDto?> Handle(
        GetProductKardexQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _context.Product
            .AsNoTracking()
            .FirstOrDefaultAsync(
                product => product.Id == request.ProductId,
                cancellationToken);

        if (product is null)
        {
            return null;
        }

        var movements = await _context.MovInv
            .AsNoTracking()
            .Where(movement =>
                movement.ProductId == request.ProductId)
            .OrderBy(movement => movement.Fecha)
            .ThenBy(movement => movement.Id)
            .ToListAsync(cancellationToken);

        var totalEntradas = 0;
        var totalSalidas = 0;
        var saldoAcumulado = 0;

        var kardexMovements = new List<KardexMovementDto>();

        foreach (var movement in movements)
        {
            var isEntrada = movement.Tipo.Equals(
                "Entrada",
                StringComparison.OrdinalIgnoreCase);

            var isSalida = movement.Tipo.Equals(
                "Salida",
                StringComparison.OrdinalIgnoreCase);

            if (!isEntrada && !isSalida)
            {
                continue;
            }

            if (isEntrada)
            {
                totalEntradas += movement.Cantidad;
                saldoAcumulado += movement.Cantidad;
            }
            else
            {
                totalSalidas += movement.Cantidad;
                saldoAcumulado -= movement.Cantidad;
            }

            kardexMovements.Add(new KardexMovementDto
            {
                Id = movement.Id,
                Fecha = movement.Fecha,
                Tipo = isEntrada ? "Entrada" : "Salida",
                Cantidad = movement.Cantidad,
                Entrada = isEntrada ? movement.Cantidad : 0,
                Salida = isSalida ? movement.Cantidad : 0,
                SaldoAcumulado = saldoAcumulado,
                Referencia = movement.Referencia
            });
        }

        return new ProductKardexDto
        {
            ProductId = product.Id,
            Codigo = product.Codigo,
            Nombre = product.Nombre,
            TotalEntradas = totalEntradas,
            TotalSalidas = totalSalidas,
            StockFinal = saldoAcumulado,
            Movimientos = kardexMovements
        };
    }
}