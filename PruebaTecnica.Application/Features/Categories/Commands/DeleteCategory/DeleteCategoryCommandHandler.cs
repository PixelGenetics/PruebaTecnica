using MediatR;
using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Common.Interfaces;

namespace PruebaTecnica.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler
    : IRequestHandler<DeleteCategoryCommand, DeleteCategoryResult>
{
    private readonly IAppDbContext _context;

    public DeleteCategoryCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<DeleteCategoryResult> Handle(
        DeleteCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _context.Category
            .FirstOrDefaultAsync(
                category => category.Id == request.Id,
                cancellationToken);

        if (category is null)
        {
            return new DeleteCategoryResult
            {
                Success = false
            };
        }

        var hasProducts = await _context.Product
            .AnyAsync(
                product => product.CategoryId == request.Id,
                cancellationToken);

        if (hasProducts)
        {
            return new DeleteCategoryResult
            {
                Success = false,
                HasProducts = true
            };
        }

        _context.Category.Remove(category);

        await _context.SaveChangesAsync(cancellationToken);

        return new DeleteCategoryResult
        {
            Success = true
        };
    }
}