using MediatR;
using PruebaTecnica.Application.Features.Categories.DTOs;

namespace PruebaTecnica.Application.Features.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(
    int Id,
    string Nombre,
    bool Estado
) : IRequest<CategoryDto?>;