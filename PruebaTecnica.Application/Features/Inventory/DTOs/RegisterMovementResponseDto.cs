namespace PruebaTecnica.Application.Features.Inventory.DTOs;

public sealed class RegisterMovementResponseDto
{
    public string Message { get; set; } = string.Empty;
    public int StockActual { get; set; }
    public InventoryMovementDto Movimiento { get; set; } = null!;
}

public sealed class RegisterMovementErrorDto
{
    public string Message { get; set; } = string.Empty;
    public int StockActual { get; set; }
}

public sealed class InventoryMovementDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public DateTime Fecha { get; set; }
    public string? Referencia { get; set; }
}
