using MediatR;
using PruebaTecnica.Application.Features.Categories.DTOs;

namespace PruebaTecnica.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(
    string Nombre,
    bool Estado
) : IRequest<CategoryDto>;