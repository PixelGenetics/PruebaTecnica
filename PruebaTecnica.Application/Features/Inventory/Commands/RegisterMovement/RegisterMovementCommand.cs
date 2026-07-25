using MediatR;

namespace PruebaTecnica.Application.Features.Inventory.Commands.RegisterMovement;
    public record RegisterMovementCommand(
        int ProductId,
        string Tipo,
        int Cantidad,
        string Referencia
    ) : IRequest<RegisterMovementResult>;
