namespace PruebaTecnica.Application.Features.Products.DTOs;

public class PagedProductsDto
{
    public int PaginaActual { get; set; }

    public int CantidadPorPagina { get; set; }

    public int TotalRegistros { get; set; }

    public int TotalPaginas { get; set; }

    public bool TienePaginaAnterior { get; set; }

    public bool TienePaginaSiguiente { get; set; }

    public List<ProductListItemDto> Productos { get; set; } = [];
}