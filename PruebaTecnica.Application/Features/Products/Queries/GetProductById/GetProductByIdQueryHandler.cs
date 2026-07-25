using MediatR;
using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Common.Interfaces;
using PruebaTecnica.Application.Features.Products.DTOs;
using PruebaTecnica.Application.Features.Products.Mappers;

namespace PruebaTecnica.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IAppDbContext _context;

    public GetProductByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDto?> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _context.Product
            .AsNoTracking()
            .Include(product => product.Category)
            .FirstOrDefaultAsync(
                product => product.Id == request.Id,
                cancellationToken);

        return product is null
            ? null
            : ProductMapper.ToDto(product);
    }
}