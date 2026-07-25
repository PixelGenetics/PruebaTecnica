using MediatR;
using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Common.Interfaces;
using PruebaTecnica.Application.Features.Categories.DTOs;
using PruebaTecnica.Application.Features.Categories.Mappers;

namespace PruebaTecnica.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler
    : IRequestHandler<UpdateCategoryCommand, CategoryDto?>
{
    private readonly IAppDbContext _context;

    public UpdateCategoryCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryDto?> Handle(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _context.Category
            .FirstOrDefaultAsync(
                category => category.Id == request.Id,
                cancellationToken);

        if (category is null)
        {
            return null;
        }

        var nombre = request.Nombre.Trim();

        var duplicatedName = await _context.Category
            .AnyAsync(
                existingCategory =>
                    existingCategory.Id != request.Id &&
                    existingCategory.Nombre == nombre,
                cancellationToken);

        if (duplicatedName)
        {
            throw new InvalidOperationException(
                "Ya existe otra categoría con ese nombre.");
        }

        category.Nombre = nombre;
        category.Estado = request.Estado;

        await _context.SaveChangesAsync(cancellationToken);

        return CategoryMapper.ToDto(category);
    }
}