using MediatR;
using PruebaTecnica.Application.Features.Categories.DTOs;

namespace PruebaTecnica.Application.Features.Categories.Queries.GetAllCategories;

public record GetAllCategoriesQuery : IRequest<List<CategoryDto>>;