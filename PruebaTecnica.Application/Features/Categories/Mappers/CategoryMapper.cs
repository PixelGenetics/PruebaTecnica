using PruebaTecnica.Application.Features.Categories.DTOs;
using PruebaTecnica.Domain.Entities;

namespace PruebaTecnica.Application.Features.Categories.Mappers;

public static class CategoryMapper
{
    public static CategoryDto ToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Nombre = category.Nombre,
            Estado = category.Estado
        };
    }
}