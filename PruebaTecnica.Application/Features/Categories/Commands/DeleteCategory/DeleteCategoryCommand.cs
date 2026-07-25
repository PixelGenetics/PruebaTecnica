using MediatR;

namespace PruebaTecnica.Application.Features.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(int Id) : IRequest<DeleteCategoryResult>;