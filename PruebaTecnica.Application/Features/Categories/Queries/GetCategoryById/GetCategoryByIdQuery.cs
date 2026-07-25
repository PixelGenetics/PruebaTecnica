using MediatR;
using PruebaTecnica.Application.Features.Categories.DTOs;

namespace PruebaTecnica.Application.Features.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(int Id) : IRequest<CategoryDto?>;