namespace PruebaTecnica.Application.Features.Inventory.DTOs;

public class KardexMovementDto
{
    public int Id { get; set; }

    public DateTime Fecha { get; set; }

    public string Tipo { get; set; } = string.Empty;

    public int Cantidad { get; set; }

    public int Entrada { get; set; }

    public int Salida { get; set; }

    public int SaldoAcumulado { get; set; }

    public string Referencia { get; set; } = string.Empty;
}