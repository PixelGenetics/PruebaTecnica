using MediatR;
using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Common.Interfaces;

namespace PruebaTecnica.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler
    : IRequestHandler<DeleteProductCommand, DeleteProductResult>
{
    private readonly IAppDbContext _context;

    public DeleteProductCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<DeleteProductResult> Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _context.Product
            .FirstOrDefaultAsync(
                product => product.Id == request.Id,
                cancellationToken);

        if (product is null)
        {
            return new DeleteProductResult
            {
                ProductNotFound = true
            };
        }

        var hasMovements = await _context.MovInv
            .AnyAsync(
                movement => movement.ProductId == request.Id,
                cancellationToken);

        if (hasMovements)
        {
            return new DeleteProductResult
            {
                HasMovements = true
            };
        }

        _context.Product.Remove(product);

        await _context.SaveChangesAsync(cancellationToken);

        return new DeleteProductResult
        {
            Success = true
        };
    }
}