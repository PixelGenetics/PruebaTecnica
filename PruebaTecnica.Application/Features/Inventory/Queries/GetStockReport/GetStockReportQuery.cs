using MediatR;
using PruebaTecnica.Application.Features.Inventory.DTOs;

namespace PruebaTecnica.Application.Features.Inventory.Queries.GetStockReport;

public record GetStockReportQuery(
    int? CategoryId,
    int? Threshold
) : IRequest<List<StockReportItemDto>>;