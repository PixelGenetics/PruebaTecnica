using MediatR;

namespace PruebaTecnica.Application.Features.Products.Commands.ChangeProductStatus;

public record ChangeProductStatusCommand(
    int Id,
    bool Estado
) : IRequest<ChangeProductStatusResult>;