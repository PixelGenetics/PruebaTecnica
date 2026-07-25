namespace PruebaTecnica.Application.Features.Inventory.DTOs;

public class MovementDto
{
    public int Id { get; set; }

    public string Tipo { get; set; } = string.Empty;

    public int Cantidad { get; set; }

    public DateTime Fecha { get; set; }

    public string Referencia { get; set; } = string.Empty;
}