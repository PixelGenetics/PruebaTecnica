using MediatR;
using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Common.Interfaces;
using PruebaTecnica.Application.Features.Products.DTOs;
using PruebaTecnica.Application.Features.Products.Mappers;

namespace PruebaTecnica.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler
    : IRequestHandler<UpdateProductCommand, ProductDto?>
{
    private readonly IAppDbContext _context;

    public UpdateProductCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDto?> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _context.Product
            .Include(product => product.Category)
            .FirstOrDefaultAsync(
                product => product.Id == request.Id,
                cancellationToken);

        if (product is null)
        {
            return null;
        }

        var codigo = request.Codigo.Trim();
        var nombre = request.Nombre.Trim();

        var duplicatedCode = await _context.Product
            .AnyAsync(
                existingProduct =>
                    existingProduct.Id != request.Id &&
                    existingProduct.Codigo == codigo,
                cancellationToken);

        if (duplicatedCode)
        {
            throw new InvalidOperationException(
                "Ya existe otro producto con ese código.");
        }

        product.Codigo = codigo;
        product.Nombre = nombre;
        product.Precio = request.Precio;
        product.Estado = request.Estado;

        await _context.SaveChangesAsync(cancellationToken);

        return ProductMapper.ToDto(product);
    }
}