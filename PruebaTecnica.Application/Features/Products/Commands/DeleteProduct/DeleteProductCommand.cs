using MediatR;

namespace PruebaTecnica.Application.Features.Products.Commands.DeleteProduct;

public record DeleteProductCommand(int Id)
    : IRequest<DeleteProductResult>;