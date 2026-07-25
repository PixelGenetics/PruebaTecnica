namespace PruebaTecnica.Application.Features.Inventory.DTOs;

public class StockReportItemDto
{
    public int ProductId { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public string Categoria { get; set; } = string.Empty;

    public bool Estado { get; set; }

    public int TotalEntradas { get; set; }

    public int TotalSalidas { get; set; }

    public int StockActual { get; set; }
}