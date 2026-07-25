using MediatR;
using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Common.Interfaces;
using PruebaTecnica.Application.Features.Categories.DTOs;
using PruebaTecnica.Application.Features.Categories.Mappers;
using PruebaTecnica.Domain.Entities;

namespace PruebaTecnica.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler
    : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly IAppDbContext _context;

    public CreateCategoryCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryDto> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var nombre = request.Nombre.Trim();

        var categoryExists = await _context.Category
            .AnyAsync(
                category => category.Nombre == nombre,
                cancellationToken);

        if (categoryExists)
        {
            throw new InvalidOperationException(
                "Ya existe una categoría con ese nombre.");
        }

        var category = new Category
        {
            Nombre = nombre,
            Estado = request.Estado
        };

        await _context.Category.AddAsync(
            category,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return CategoryMapper.ToDto(category);
    }
}