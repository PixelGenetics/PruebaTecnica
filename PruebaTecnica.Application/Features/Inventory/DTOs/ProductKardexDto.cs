namespace PruebaTecnica.Application.Features.Inventory.DTOs;

public class ProductKardexDto
{
    public int ProductId { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public int TotalEntradas { get; set; }

    public int TotalSalidas { get; set; }

    public int StockFinal { get; set; }

    public List<KardexMovementDto> Movimientos { get; set; } = [];
}