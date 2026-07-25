using MediatR;
using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Common.Interfaces;

namespace PruebaTecnica.Application.Features.Products.Commands.ChangeProductStatus;

public class ChangeProductStatusCommandHandler
    : IRequestHandler<ChangeProductStatusCommand, ChangeProductStatusResult>
{
    private readonly IAppDbContext _context;

    public ChangeProductStatusCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<ChangeProductStatusResult> Handle(
        ChangeProductStatusCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _context.Product
            .FirstOrDefaultAsync(
                product => product.Id == request.Id,
                cancellationToken);

        if (product is null)
        {
            return new ChangeProductStatusResult
            {
                ProductNotFound = true
            };
        }

        if (product.Estado == request.Estado)
        {
            return new ChangeProductStatusResult
            {
                ErrorMessage = request.Estado
                    ? "El producto ya está activo."
                    : "El producto ya está desactivado."
            };
        }

        if (request.Estado && product.Precio <= 0)
        {
            return new ChangeProductStatusResult
            {
                ErrorMessage =
                    "No se puede activar el producto porque su precio debe ser mayor que cero."
            };
        }

        product.Estado = request.Estado;

        await _context.SaveChangesAsync(cancellationToken);

        return new ChangeProductStatusResult
        {
            Success = true,
            Message = product.Estado
                ? "Producto activado correctamente."
                : "Producto desactivado correctamente.",
            Id = product.Id,
            Codigo = product.Codigo,
            Nombre = product.Nombre,
            Precio = product.Precio,
            Estado = product.Estado
        };
    }
}