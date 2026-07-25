namespace PruebaTecnica.Application.Features.Categories.DTOs;

public class CategoryDto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public bool Estado { get; set; }
}