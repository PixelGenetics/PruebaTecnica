using MediatR;
using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Common.Interfaces;
using PruebaTecnica.Application.Features.Products.DTOs;

namespace PruebaTecnica.Application.Features.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler
    : IRequestHandler<GetAllProductsQuery, PagedProductsDto>
{
    private readonly IAppDbContext _context;

    public GetAllProductsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedProductsDto> Handle(
        GetAllProductsQuery request,
        CancellationToken cancellationToken)
    {
        var pagina = request.Pagina < 1
            ? 1
            : request.Pagina;

        var cantidadPorPagina = request.CantidadPorPagina;

        if (cantidadPorPagina < 1)
        {
            cantidadPorPagina = 10;
        }

        if (cantidadPorPagina > 100)
        {
            cantidadPorPagina = 100;
        }

        var query = _context.Product
            .AsNoTracking()
            .Include(product => product.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Nombre))
        {
            query = query.Where(product =>
                product.Nombre.Contains(request.Nombre));
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(product =>
                product.CategoryId == request.CategoryId.Value);
        }

        if (request.Estado.HasValue)
        {
            query = query.Where(product =>
                product.Estado == request.Estado.Value);
        }

        var totalRegistros = await query.CountAsync(cancellationToken);

        var direccionDescendente =
            request.Direccion.Equals(
                "desc",
                StringComparison.OrdinalIgnoreCase);

        query = request.OrdenarPor.ToLower() switch
        {
            "codigo" => direccionDescendente
                ? query.OrderByDescending(product => product.Codigo)
                : query.OrderBy(product => product.Codigo),

            "precio" => direccionDescendente
                ? query.OrderByDescending(product => product.Precio)
                : query.OrderBy(product => product.Precio),

            "categoria" => direccionDescendente
                ? query.OrderByDescending(product => product.Category.Nombre)
                : query.OrderBy(product => product.Category.Nombre),

            "estado" => direccionDescendente
                ? query.OrderByDescending(product => product.Estado)
                : query.OrderBy(product => product.Estado),

            "fechacreacion" => direccionDescendente
                ? query.OrderByDescending(product => product.FechaCreacion)
                : query.OrderBy(product => product.FechaCreacion),

            _ => direccionDescendente
                ? query.OrderByDescending(product => product.Nombre)
                : query.OrderBy(product => product.Nombre)
        };

        var productos = await query
            .Skip((pagina - 1) * cantidadPorPagina)
            .Take(cantidadPorPagina)
            .Select(product => new ProductListItemDto
            {
                Id = product.Id,
                Codigo = product.Codigo,
                Nombre = product.Nombre,
                CategoryId = product.CategoryId,
                Categoria = product.Category.Nombre,
                Precio = product.Precio,
                Estado = product.Estado,
                FechaCreacion = product.FechaCreacion
            })
            .ToListAsync(cancellationToken);

        var totalPaginas = (int)Math.Ceiling(
            totalRegistros / (double)cantidadPorPagina);

        return new PagedProductsDto
        {
            PaginaActual = pagina,
            CantidadPorPagina = cantidadPorPagina,
            TotalRegistros = totalRegistros,
            TotalPaginas = totalPaginas,
            TienePaginaAnterior = pagina > 1,
            TienePaginaSiguiente = pagina < totalPaginas,
            Productos = productos
        };
    }
}