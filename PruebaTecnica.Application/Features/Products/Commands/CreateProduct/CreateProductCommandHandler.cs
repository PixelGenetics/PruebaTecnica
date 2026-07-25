using MediatR;
using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Common.Interfaces;
using PruebaTecnica.Application.Features.Products.DTOs;
using PruebaTecnica.Application.Features.Products.Mappers;
using PruebaTecnica.Domain.Entities;

namespace PruebaTecnica.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IAppDbContext _context;

    public CreateProductCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDto> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var codigo = request.Codigo.Trim();
        var nombre = request.Nombre.Trim();

        var categoryExists = await _context.Category
            .AnyAsync(
                category => category.Id == request.CategoryId,
                cancellationToken);

        if (!categoryExists)
        {
            throw new KeyNotFoundException(
                "La categoría indicada no existe.");
        }

        var codeExists = await _context.Product
            .AnyAsync(
                product => product.Codigo == codigo,
                cancellationToken);

        if (codeExists)
        {
            throw new InvalidOperationException(
                "Ya existe un producto con ese código.");
        }

        var product = new Product
        {
            Codigo = codigo,
            Nombre = nombre,
            CategoryId = request.CategoryId,
            Precio = request.Precio,
            Estado = request.Estado,
            FechaCreacion = DateTime.Now
        };

        await _context.Product.AddAsync(
            product,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        product.Category = await _context.Category
            .AsNoTracking()
            .FirstAsync(
                category => category.Id == product.CategoryId,
                cancellationToken);

        return ProductMapper.ToDto(product);
    }
}