using MediatR;
using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Common.Interfaces;
using PruebaTecnica.Application.Features.Inventory.DTOs;

namespace PruebaTecnica.Application.Features.Inventory.Queries.GetStockReport;

public class GetStockReportHandler
    : IRequestHandler<GetStockReportQuery, List<StockReportItemDto>>
{
    private readonly IAppDbContext _context;

    public GetStockReportHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<StockReportItemDto>> Handle(
        GetStockReportQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Product
            .AsNoTracking()
            .AsQueryable();

        if (request.CategoryId.HasValue)
        {
            query = query.Where(product =>
                product.CategoryId == request.CategoryId.Value);
        }

        var report = await query
            .Select(product => new StockReportItemDto
            {
                ProductId = product.Id,
                Codigo = product.Codigo,
                Nombre = product.Nombre,
                CategoryId = product.CategoryId,

                Categoria = product.Category != null
                    ? product.Category.Nombre
                    : string.Empty,

                Estado = product.Estado,

                TotalEntradas = product.Movimientos
                    .Where(movement =>
                        movement.Tipo.ToLower() == "entrada")
                    .Sum(movement =>
                        (int?)movement.Cantidad) ?? 0,

                TotalSalidas = product.Movimientos
                    .Where(movement =>
                        movement.Tipo.ToLower() == "salida")
                    .Sum(movement =>
                        (int?)movement.Cantidad) ?? 0,

                StockActual =
                    (
                        product.Movimientos
                            .Where(movement =>
                                movement.Tipo.ToLower() == "entrada")
                            .Sum(movement =>
                                (int?)movement.Cantidad) ?? 0
                    )
                    -
                    (
                        product.Movimientos
                            .Where(movement =>
                                movement.Tipo.ToLower() == "salida")
                            .Sum(movement =>
                                (int?)movement.Cantidad) ?? 0
                    )
            })
            .ToListAsync(cancellationToken);

        if (request.Threshold.HasValue)
        {
            report = report
                .Where(product =>
                    product.StockActual < request.Threshold.Value)
                .ToList();
        }

        return report
            .OrderBy(product => product.StockActual)
            .ThenBy(product => product.Nombre)
            .ToList();
    }
}