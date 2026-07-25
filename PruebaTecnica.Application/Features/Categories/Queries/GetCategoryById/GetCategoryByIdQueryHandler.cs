using MediatR;
using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Common.Interfaces;
using PruebaTecnica.Application.Features.Categories.DTOs;
using PruebaTecnica.Application.Features.Categories.Mappers;

namespace PruebaTecnica.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler
    : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
{
    private readonly IAppDbContext _context;

    public GetCategoryByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryDto?> Handle(
        GetCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var category = await _context.Category
            .AsNoTracking()
            .FirstOrDefaultAsync(
                category => category.Id == request.Id,
                cancellationToken);

        return category is null
            ? null
            : CategoryMapper.ToDto(category);
    }
}