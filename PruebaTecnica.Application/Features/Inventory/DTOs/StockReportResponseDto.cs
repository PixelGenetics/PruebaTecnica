using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTecnica.Application.Features.Inventory.DTOs;

public sealed class StockReportResponseDto
{
    public int? CategoryId { get; set; }

    public int? Threshold { get; set; }

    public int TotalRegistros { get; set; }

    public IReadOnlyCollection<StockReportItemDto> Productos { get; set; }
        = Array.Empty<StockReportItemDto>();

}

