using System.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Common.Interfaces;
using PruebaTecnica.Domain.Entities;

namespace PruebaTecnica.Application.Features.Inventory.Commands.RegisterMovement;

public class RegisterMovementHandler
    : IRequestHandler<RegisterMovementCommand, RegisterMovementResult>
{
    private readonly IAppDbContext _context;

    public RegisterMovementHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<RegisterMovementResult> Handle(
        RegisterMovementCommand request,
        CancellationToken cancellationToken)
    {
        var tipo = request.Tipo?.Trim();

        if (string.IsNullOrWhiteSpace(tipo))
        {
            return new RegisterMovementResult(
                false,
                "El tipo de movimiento es obligatorio.",
                0);
        }

        string? tipoNormalizado = tipo.Equals(
            "Entrada",
            StringComparison.OrdinalIgnoreCase)
                ? "Entrada"
                : tipo.Equals(
                    "Salida",
                    StringComparison.OrdinalIgnoreCase)
                        ? "Salida"
                        : null;

        if (tipoNormalizado is null)
        {
            return new RegisterMovementResult(
                false,
                "El tipo de movimiento debe ser 'Entrada' o 'Salida'.",
                0);
        }

        if (request.Cantidad <= 0)
        {
            return new RegisterMovementResult(
                false,
                "La cantidad debe ser mayor que cero.",
                0);
        }

        if (string.IsNullOrWhiteSpace(request.Referencia))
        {
            return new RegisterMovementResult(
                false,
                "La referencia es obligatoria.",
                0);
        }

        var productExists = await _context.Product
            .AsNoTracking()
            .AnyAsync(
                product => product.Id == request.ProductId,
                cancellationToken);

        if (!productExists)
        {
            return new RegisterMovementResult(
                false,
                "El producto indicado no existe.",
                0);
        }

        await using var transaction =
            await _context.BeginTransactionAsync(
                IsolationLevel.Serializable,
                cancellationToken);

        try
        {
            var totalEntradas = await _context.MovInv
                .Where(movement =>
                    movement.ProductId == request.ProductId &&
                    movement.Tipo == "Entrada")
                .SumAsync(
                    movement => (int?)movement.Cantidad,
                    cancellationToken) ?? 0;

            var totalSalidas = await _context.MovInv
                .Where(movement =>
                    movement.ProductId == request.ProductId &&
                    movement.Tipo == "Salida")
                .SumAsync(
                    movement => (int?)movement.Cantidad,
                    cancellationToken) ?? 0;

            var stockActual = totalEntradas - totalSalidas;

            if (tipoNormalizado == "Salida" && stockActual <= 0)
            {
                await transaction.RollbackAsync(cancellationToken);

                return new RegisterMovementResult(
                    false,
                    $"No se puede registrar una salida. " +
                    $"El stock disponible es {stockActual}.",
                    stockActual);
            }

            if (tipoNormalizado == "Salida" &&
                request.Cantidad > stockActual)
            {
                await transaction.RollbackAsync(cancellationToken);

                return new RegisterMovementResult(
                    false,
                    $"Stock insuficiente. Disponible: {stockActual}. " +
                    $"Salida solicitada: {request.Cantidad}.",
                    stockActual);
            }

            var nuevoStock = tipoNormalizado == "Entrada"
                ? stockActual + request.Cantidad
                : stockActual - request.Cantidad;

            if (tipoNormalizado == "Salida" && nuevoStock < 0)
            {
                await transaction.RollbackAsync(cancellationToken);

                return new RegisterMovementResult(
                    false,
                    "La salida dejaría el stock en negativo.",
                    stockActual);
            }

            var movimiento = new MovInv
            {
                ProductId = request.ProductId,
                Tipo = tipoNormalizado,
                Cantidad = request.Cantidad,
                Referencia = request.Referencia.Trim()
            };

            await _context.MovInv.AddAsync(
                movimiento,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new RegisterMovementResult(
                true,
                "Movimiento registrado correctamente.",
                nuevoStock,
                movimiento);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}