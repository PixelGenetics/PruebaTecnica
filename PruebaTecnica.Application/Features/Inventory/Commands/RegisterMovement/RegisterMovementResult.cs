using PruebaTecnica.Domain.Entities;

namespace PruebaTecnica.Application.Features.Inventory.Commands.RegisterMovement;

public record RegisterMovementResult(
    bool Success,
    string Message,
    int StockActual,
    MovInv? Movimiento = null
);